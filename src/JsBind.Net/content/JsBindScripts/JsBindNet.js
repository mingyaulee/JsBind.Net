(function () {
  let onReadyCallbacks = [];
  globalThis.JsBindNet = {
    isReady: false,
    onReady: function (callback) { onReadyCallbacks.push(callback); }
  };

  (async () => {
    var JsBindNet = (await import("./Modules/JsBindNet.js")).default;
    globalThis.JsBindNet = new JsBindNet();
    onReadyCallbacks.forEach(callback => callback());
  })();
})();