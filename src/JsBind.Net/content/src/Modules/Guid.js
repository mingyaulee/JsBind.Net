class GuidClass {
  /**
   * Generate a new GUID.
   * @returns {string}
   */
  newGuid() {
    if (globalThis.crypto.randomUUID) {
      return globalThis.crypto.randomUUID();
    }

    return (1e7.toString() + -1e3 + -4e3 + -8e3 + -1e11).replaceAll(/[018]/g, c =>
      (Number.parseInt(c) ^ globalThis.crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> Number.parseInt(c) / 4).toString(16)
    );
  }
}

const Guid = new GuidClass();
export default Guid;