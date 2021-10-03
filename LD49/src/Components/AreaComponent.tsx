import { Colored } from "web-gash";
import { CArea } from "../Content/Constants";
import { IArea } from "../Entities/IArea";

export function AreaComponent(props: {area: IArea | string}) {
  return (
    <Colored foreground={CArea}>{typeof props.area === 'string'? props.area : props.area.name}</Colored>
  )
};