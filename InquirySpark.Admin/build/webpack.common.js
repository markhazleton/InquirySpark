const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

module.exports = {
  entry: {
    site: path.resolve(__dirname, '../src/js/site.js')
  },
  output: {
    path: path.resolve(__dirname, '../wwwroot'),
    filename: 'js/[name].js',
    publicPath: '/',
    clean: {
      keep: /lib\//  // Keep the lib folder for now during transition
    }
  },
  module: {
    rules: [
      {
        test: /\.css$/,
        use: [
          MiniCssExtractPlugin.loader,
          'css-loader'
        ]
      },
      {
        test: /\.(woff|woff2|eot|ttf|otf)$/i,
        type: 'asset/resource',
        generator: {
          filename: 'fonts/[name][ext]'
        }
      },
      {
        test: /\.(png|svg|jpg|jpeg|gif)$/i,
        type: 'asset/resource',
        generator: {
          filename: 'images/[name][ext]'
        }
      }
    ]
  },
  plugins: [
    new CleanWebpackPlugin({
      cleanOnceBeforeBuildPatterns: [
        'js/site.js',
        'js/site.min.js',
        'css/site.css',
        'css/site.min.css',
        'fonts/**/*'
      ]
    }),
    new MiniCssExtractPlugin({
      filename: 'css/[name].css'
    }),
    new CopyWebpackPlugin({
      patterns: [
        {
          from: path.resolve(__dirname, '../node_modules/bootstrap-icons/font/fonts'),
          to: path.resolve(__dirname, '../wwwroot/fonts'),
          noErrorOnMissing: true
        },
        {
          from: path.resolve(__dirname, '../node_modules/pdfmake/build/vfs_fonts.js'),
          to: path.resolve(__dirname, '../wwwroot/js/vfs_fonts.js')
        }
      ]
    })
  ],
  resolve: {
    extensions: ['.js', '.json'],
    alias: {
      'pdfmake': path.resolve(__dirname, '../node_modules/pdfmake/build/pdfmake.min.js')
    }
  },
  optimization: {
    splitChunks: false
  }
};
