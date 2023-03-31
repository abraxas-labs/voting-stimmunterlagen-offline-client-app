export interface ShellExecuteDecodedResult {
  exitCode: number;
  data: string;
}

export interface ShellExecuteBinaryResult {
  exitCode: number;
  data: Uint8Array;
}
