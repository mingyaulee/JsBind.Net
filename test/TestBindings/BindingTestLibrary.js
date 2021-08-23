(function (global) {
  const bindingTestLibrary = {
  };

  class TestClass {
    randomValue;
    constructor() {
      this.randomValue = Math.random();
    }
  }

  bindingTestLibrary.isPlainObjectPassed = (obj) => { return obj?.isTestClass === true || false };

  const testObjectReferenceReviverInstance = new TestClass();
  bindingTestLibrary.getTestObjectReviverInstanceFromFunction = () => testObjectReferenceReviverInstance;
  bindingTestLibrary.testObjectReviverInstanceFromProperty = testObjectReferenceReviverInstance;
  bindingTestLibrary.isObjectReferenceRevived = (instance) => instance === testObjectReferenceReviverInstance;
  bindingTestLibrary.isNestedObjectReferenceRevived = (nestedObject) => nestedObject.instance === testObjectReferenceReviverInstance;

  bindingTestLibrary.isDelegateReferenceRevived = (delegateReference) => delegateReference instanceof Function;
  bindingTestLibrary.isNestedDelegateReferenceRevived = (nestedObject) => nestedObject.delegate instanceof Function;
  bindingTestLibrary.areDelegateReferencesEqual = (delegateReference1, delegateReference2) => delegateReference1 === delegateReference2;

  bindingTestLibrary.testInvokeDelegate = (delegateReference, ...args) => delegateReference(...args);
  bindingTestLibrary.testInvokeDelegateAsync = async (delegateReference, ...args) => delegateReference(...args);

  function functionDelegate() { return true; }
  bindingTestLibrary.getFunctionDelegate = () => functionDelegate;
  bindingTestLibrary.getNestedActionDelegate = () => { return { action: functionDelegate }; };
  function mirrorFunctionDelegate(val) { return val; }
  bindingTestLibrary.getMirrorFunctionDelegate = () => mirrorFunctionDelegate;

  global.BindingTestLibrary = bindingTestLibrary;
})(globalThis);
