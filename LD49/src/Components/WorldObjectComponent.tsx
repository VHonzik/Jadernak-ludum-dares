import { Colored } from "web-gash";
import { CWorldObject } from "../Content/Constants";
import { IWorldObject } from "../Entities/IWorldObject";

export function WorldObjectComponent(props: {worldObject: IWorldObject | string}) {
  return (
    <Colored foreground={CWorldObject}>{typeof props.worldObject === 'string'? props.worldObject : props.worldObject.name}</Colored>
  )
}