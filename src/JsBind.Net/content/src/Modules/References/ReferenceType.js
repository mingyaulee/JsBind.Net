/** 
 * @typedef {"Object" | "Delegate"} ReferenceTypeEnumValue
 */

/**
 * @typedef ReferenceTypeEnum
 * @property {ReferenceTypeEnumValue} Object
 * @property {ReferenceTypeEnumValue} Delegate
 */

/**
 * @type {ReferenceTypeEnum}
 */
const ReferenceType = {
  Object: "Object",
  Delegate: "Delegate"
};

export default ReferenceType;