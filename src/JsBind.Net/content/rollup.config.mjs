export default [
  {
    input: 'src/JsBindNetScript.js',
    output: {
      file: 'dist/JsBindNet.js',
      format: 'iife' // immediately-invoked function expression — suitable for <script> tags
    }
  },
  {
    input: 'src/JsBind.Net.lib.module.js',
    output: {
      file: 'dist/JsBind.Net.lib.module.js',
      format: 'es'
    }
  }
];