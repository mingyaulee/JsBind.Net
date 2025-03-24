import JsBindNet from "./Modules/JsBindNet.js";

export async function beforeStart() {
  globalThis.JsBindNet = new JsBindNet();
}