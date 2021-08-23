﻿export default class ObjectBindingConfiguration {
  /** @type {string[]} */
  include;
  /** @type {string[]} */
  exclude;
  /** @type {Object<string, ObjectBindingConfiguration>} */
  propertyBindings;
  /** @type {boolean} */
  setAccessPath;
  /** @type {ObjectBindingConfiguration} */
  arrayItemBinding;
}