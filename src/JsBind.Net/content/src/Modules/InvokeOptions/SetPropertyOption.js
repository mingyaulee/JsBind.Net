import InvokeOption from "./InvokeOption.js";

export default class SetPropertyOption extends InvokeOption {
  /** @type {string} */
  accessPath;
  /** @type {string} */
  propertyName;
  /** @type {any} */
  propertyValue;
}