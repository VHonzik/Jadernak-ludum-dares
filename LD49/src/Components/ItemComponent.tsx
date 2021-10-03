import { Colored } from "web-gash";
import { CItem } from "../Content/Constants";
import { IItem } from "../Entities/IItem";

export function ItemComponent(props: {item: IItem | string}) {
  return (
    <Colored foreground={CItem}>{typeof props.item === 'string'? props.item : props.item.name}</Colored>
  );
}