import * as EnvDataset from "../../../assets/env.json";

export interface IEnvironment {
  coreBaseUrl: string;
}

export const Environment_Ver: IEnvironment = (EnvDataset as any).default;
