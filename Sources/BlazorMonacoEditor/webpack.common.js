const path = require('path');
const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

module.exports = {
  entry: {
     monaco: "./wwwroot/ts/MonacoInterop.ts",
     scriptLoader: "./wwwroot/ts/scriptLoader.ts",
    "editor.worker": "monaco-editor/esm/vs/editor/editor.worker.js"
  },
  resolve: {
    extensions: [".ts", ".js"]
  },
  output: {
    globalObject: "self",
    filename: "[name].bundle.js",
    path: path.resolve(__dirname, 'wwwroot/js'),
  },
  module: {
    rules: [
    {
        test: /\.ts?$/,
        use: "ts-loader",
        exclude: /node_modules/
    },
    {
      test: /\.css$/,
      use: ['style-loader', 'css-loader']
    }, 
    {
      test: /\.ttf$/,
      type: 'asset/resource',
    }]
  },
  plugins: [
    new MonacoWebpackPlugin({publicPath: '/_content/Blazor.Monaco/'})
  ]
};