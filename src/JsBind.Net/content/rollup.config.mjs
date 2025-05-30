import rollupNormalizeLineEndings from "./rollup-plugins/rollupNormalizeLineEndings.mjs";

export default [
  {
    input: 'src/JsBindNetScript.js',
    output: {
      file: 'dist/JsBindNet.js',
      format: 'iife' // immediately-invoked function expression — suitable for <script> tags
    },
    plugins: [
      rollupNormalizeLineEndings()
    ]
  },
  {
    input: 'src/JsBind.Net.lib.module.js',
    output: {
      file: 'dist/JsBind.Net.lib.module.js',
      format: 'es'
    },
    plugins: [
      rollupNormalizeLineEndings()
    ]
  }
];