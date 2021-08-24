import JsObjectHandler from "../JsObjectHandler.js";
import { IsObjectReference } from "../References/ObjectReference.js";

class ObjectReferenceReviverClass {
  /**
   * Converts an ObjectReference JSON object to an object.
   * @param {any} key
   * @param {any} value
   */
  revive(key, value) {
    if (IsObjectReference(value)) {
      return JsObjectHandler.getObjectFromAccessPath(value.accessPath);
    }

    return value;
  }
}

const ObjectReferenceReviver = new ObjectReferenceReviverClass();
export default ObjectReferenceReviver;