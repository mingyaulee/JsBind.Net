import InvokeOption from "./InvokeOption.js";

export default class GetPropertyOption extends InvokeOption {
  /** @type {string} */
  accessPath;
  /** @type {string} */
  propertyName;
}