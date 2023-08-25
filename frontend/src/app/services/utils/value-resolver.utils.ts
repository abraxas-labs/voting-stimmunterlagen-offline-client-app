/**
 * Resolves a value in an object by property paths. Returns the first existing value.
 */
export function resolveValue(obj: any, propertyPaths: string[]): any {
  for (const path of propertyPaths) {
    const value = getValueFromPath(obj, path);
    if (value !== undefined) {
      return value;
    }
  }

  return undefined;
}

function getValueFromPath(obj: any, propertyPath: string): any {
  const pathElements = propertyPath.split('.');
  let groupElement = obj;
  for (const pathElement of pathElements) {
    groupElement = groupElement[pathElement];
    if (groupElement === undefined) {
      return undefined;
    }
  }
  return groupElement;
}
