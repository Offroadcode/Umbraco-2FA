
module.exports = function(grunt) {
  require('load-grunt-tasks')(grunt);
  var path = require('path');

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    pkgMeta: grunt.file.readJSON('config/meta.json'),
    dest: grunt.option('target') || 'dist',
    basePath: path.join('<%= dest %>', 'App_Plugins', '<%= pkgMeta.name %>'),

    watch: {
      options: {
        spawn: false,
        atBegin: true
      },
      dll: {
        files: ['Umbraco2FA/Umbraco/Fortress/**/*.cs'] ,
        tasks: ['msbuild:dist', 'copy:dll']
      },
      js: {
        files: ['Umbraco2FA/**/*.js'],
        tasks: ['concat:dist']
      },
      html: {
        files: ['Umbraco2FA/**/*.html'],
        tasks: ['copy:html', 'copy:treeHtml']
      },
      lang: {
        files: ['Umbraco2FA/lang/*.xml'],
        tasks: ['copy:lang']
      },
      sass: {
        files: ['Umbraco2FA/**/*.scss'],
        tasks: ['sass', 'copy:css']
      },
      css: {
        files: ['Umbraco2FA/**/*.css'],
        tasks: ['copy:css']
      },
      manifest: {
        files: ['Umbraco2FA/package.manifest'],
        tasks: ['copy:manifest']
      }
      },

    concat: {
      options: {
        stripBanners: false
      },
      dist: {
        src: [
			      'Umbraco2FA/resources/BackOffice.resource.js',
            'Umbraco2FA/resources/TwoFactor.resource.js',
            'Umbraco2FA/controllers/edit.controller.js',
            'Umbraco2FA/controllers/umbraco2FA.dashboard.controller.js',
            'Umbraco2FA/controllers/TwoFactor/umbraco2FA.twoFactorLogin.controller.js',
            'Umbraco2FA/controllers/TwoFactor/umbraco2FA.twofactor.controller.js'
        ],
        dest: '<%= basePath %>/js/Umbraco2FA.js'
      }
    },

    copy: {
      dll: {
        cwd: 'Umbraco2FA/Umbraco/Fortress/bin/debug/',
        src: ['Orc.Fortress.dll','Google.Authenticator.dll'],
        dest: '<%= dest %>/bin/',
        expand: true
      },
      html: {
        cwd: 'Umbraco2FA/views/',
        src: [
            'Dashboard.html'
        ],
        dest: '<%= basePath %>/views/',
        expand: true,
        rename: function(dest, src) {
            return dest + src;
        }
      },
      lang: {
        cwd: 'Umbraco2FA/lang',
        src: [
          'en.xml',
          'en-US.xml'
        ],
        dest: '<%= basePath %>/lang/',
        expand: true,
        rename: function(dest, src) {
            return dest + src;
        }        
      },
      treeHtml: {
        cwd: 'Umbraco2FA/views/backoffice/fortressTree/',
        src: [
            'edit.html',
            'TwoFactor.html'
        ],
        dest: '<%= basePath %>/backoffice/fortressTree/', 
        expand: true,
        rename: function(dest, src) {
            return dest + src;
        }
      },    
	  twoFAhtml: {
        cwd: 'Umbraco2FA/views/Twofactor/',
        src: [
            'Setup.html',
            'smsProvider.html',
            'TwoFactorLogin.html'
        ],
        dest: '<%= basePath %>/backoffice/TwoFactor/', 
        expand: true,
        rename: function(dest, src) {
            return dest + src;
        }
      },     
      css: {
        cwd: 'Umbraco2FA/css/',
        src: [
          'Umbraco2FA.css'
        ],
        dest: '<%= basePath %>/css/',
        expand: true,
        rename: function(dest, src) {
          return dest + src;
        }
      },
      manifest: {
          cwd: 'Umbraco2FA/',
          src: [
            'package.manifest'
          ],
          dest: '<%= basePath %>/',
          expand: true,
          rename: function(dest, src) {
            return dest + src;
          }
        },
      umbraco: {
        cwd: '<%= dest %>',
        src: '**/*',
        dest: 'tmp/umbraco',
        expand: true
      }
    },

    umbracoPackage: {
      options: {
        name: "<%= pkgMeta.name %>",
        version: '<%= pkgMeta.version %>',
        url: '<%= pkgMeta.url %>',
        license: '<%= pkgMeta.license %>',
        licenseUrl: '<%= pkgMeta.licenseUrl %>',
        author: '<%= pkgMeta.author %>',
        authorUrl: '<%= pkgMeta.authorUrl %>',
        manifest: 'config/package.xml',
        readme: 'config/readme.txt',
        sourceDir: 'tmp/umbraco',
        outputDir: 'pkg',
      }
    },

    jshint: {
      options: {
        jshintrc: '.jshintrc'
      },
      src: {
        src: ['app/**/*.js', 'lib/**/*.js']
      }
  },

  sass: {
		dist: {
			options: {
				style: 'compressed'
			},
			files: {
				'Umbraco2FA/css/Umbraco2FA.css': 'Umbraco2FA/sass/Umbraco2FA.scss'
			}
		}
	},

  clean: {
      build: '<%= grunt.config("basePath").substring(0, 4) == "dist" ? "dist/**/*" : "null" %>',
      tmp: ['tmp'],
      html: [
        'Umbraco2FA/views/*.html',
        '!Umbraco2FA/views/Dashboard.html'
      ],
      css: [
        'Umbraco2FA/css/*.css',
        '!Umbraco2FA/css/Umbraco2FA.css'
      ],
	    sass: [
		    'Umbraco2FA/sass/*.scss',
		    '!Umbraco2FA/sass/Umbraco2FA.scss'
	    ]
  },
  msbuild: {
      options: {
        stdout: true,
        verbosity: 'normal',
        maxCpuCount: 4,
        version: 4.0,
        buildParameters: {
          WarningLevel: 2,
          NoWarn: 1607
        }
    },
    dist: {
        src: ['Umbraco2FA/Umbraco/Fortress/Orc.Fortress.csproj'],
        options: {
            projectConfiguration: 'Debug',
            targets: ['Clean', 'Rebuild'],
        }
    }
  }

});

grunt.registerTask('default', [
  'concat', 
  'sass:dist', 
  'copy:html', 
  'copy:lang', 
  'copy:treeHtml', 
  'copy:twoFAhtml', 
  'copy:manifest', 
  'copy:css', 
  'msbuild:dist', 
  'copy:dll', 
  'clean:html',  
  'clean:sass', 
  'clean:css']);  

grunt.registerTask('umbraco', [
  'clean:tmp', 
  'default', 
  'copy:umbraco', 
  'umbracoPackage', 
  'clean:tmp']);
};
