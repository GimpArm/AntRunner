import { GameEvent, Item, ItemColor, AntAction, EchoResponse, GameState, Ant, DamageValues, ItemBonusValues } from "antrunner";
import MapPosition from "./MapPosition";


export default class ExampleAnt extends Ant {
    public Name: string = "TypeScript Ant";
    public Action: AntAction;
    private myColor: ItemColor;

    private mapWidth: number;
    private mapHeight: number;

    private currentX: number;
    private currentY: number;

    private currentMode: number;
    private lastAction: AntAction;
    private map: MapPosition[][];

    private searchPrimary: AntAction;
    private searchSecondary: AntAction;
    
    public Initialize(mapWidth: number, mapHeight: number, color: ItemColor, startX: number, startY: number) {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.myColor = color;
        this.currentX = startX;
        this.currentY = startY;

        this.currentMode = 0;
        this.lastAction = AntAction.Wait;
        this.map = [];

        for (let y = 0; y < this.mapHeight; ++y) {
            this.map[y] = [];
            for (let x = 0; x < this.mapWidth; ++x) {
                this.map[y][x] = new MapPosition();
            }
        }
        this.map[startY][startX].Known = true;
        this.setSearchDirection();
    }

    public Tick(state: GameState) {
        this.processEcho(state.Response);
        this.processGameEvent(state.Event);

        if (this.currentMode === 0) {
            this.mapMode();
        }
    }

    private setAction(a: AntAction) {
        this.Action = a;
        this.lastAction = a;
    }

    private processEcho(response: EchoResponse) {
        if (!response) return;
        
        switch (this.lastAction) {
            case AntAction.EchoRight:
                for (let i = 1; i < response.Distance; ++i) {
                    this.map[this.currentY][this.currentX + i].Known = true;
                }
                if (this.currentX + response.Distance < this.mapWidth) {
                    this.map[this.currentY][this.currentX + response.Distance].Known = true;
                    this.map[this.currentY][this.currentX + response.Distance].Item = response.Item;
                }
                break;
            case AntAction.EchoDown:
                for (let i = 1; i < response.Distance; ++i) {
                    this.map[this.currentY + i][this.currentX].Known = true;
                }
                if (this.currentY + response.Distance < this.mapHeight) {
                    this.map[this.currentY + response.Distance][this.currentX].Known = true;
                    this.map[this.currentY + response.Distance][this.currentX].Item = response.Item;
                }
                break;
            case AntAction.EchoLeft:
                for (let i = 1; i < response.Distance; ++i) {
                    this.map[this.currentY][this.currentX - i].Known = true;
                }
                if (this.currentX - response.Distance >= 0) {
                    this.map[this.currentY][this.currentX - response.Distance].Known = true;
                    this.map[this.currentY][this.currentX - response.Distance].Item = response.Item;
                }
                break;
            case AntAction.EchoUp:
                for (let i = 1; i < response.Distance; ++i) {
                    this.map[this.currentY - i][this.currentX].Known = true;
                }
                if (this.currentY - response.Distance >= 0) {
                    this.map[this.currentY - response.Distance][this.currentX].Known = true;
                    this.map[this.currentY - response.Distance][this.currentX].Item = response.Item;
                }
                break;
            default:
                //Should never come here
                break;
        }
    }

    private processGameEvent(e) {
        if (e & GameEvent.CollisionDamage) return;

        switch (this.lastAction) {
            case AntAction.MoveRight:
                this.currentX++;
                break;
            case AntAction.MoveDown:
                this.currentY++;
                break;
            case AntAction.MoveLeft:
                this.currentX--;
                break;
            case AntAction.MoveUp:
                this.currentY--;
                break;
        }
    }

    private mapMode() {
        //Check left
        if (this.currentX > 0 && !this.map[this.currentY][this.currentX - 1].Known) {
            this.setAction(AntAction.EchoLeft);
            return;
        }

        //Check right
        if (this.currentX < this.mapWidth - 1 && !this.map[this.currentY][this.currentX + 1].Known) {
            this.setAction(AntAction.EchoRight);
            return;
        }

        //Check up
        if (this.currentY > 0 && !this.map[this.currentY - 1][this.currentX].Known) {
            this.setAction(AntAction.EchoUp);
            return;
        }

        //Check down
        if (this.currentY < this.mapHeight - 1 && !this.map[this.currentY + 1][this.currentX].Known) {
            this.setAction(AntAction.EchoDown);
            return;
        }

        //All tiles next to us are known, move a direction
        if (this.canMove(this.searchPrimary)) {
            this.setAction(this.searchPrimary);
            return;
        }

        if (this.canMove(this.searchSecondary)) {
            this.setAction(this.searchSecondary);
            return;
        }

        if (this.lastAction !== this.searchPrimary && this.canMove(this.oppositeDirection(this.searchPrimary))) {
            this.searchPrimary = this.oppositeDirection(this.searchPrimary);
            this.setAction(this.searchPrimary);
            return;
        }

        if (this.canMove(this.oppositeDirection(this.searchSecondary))) {
            this.searchSecondary = this.oppositeDirection(this.searchSecondary);
            this.setAction(this.searchSecondary);
            return;
        }

        this.setSearchDirection();
    }

    private setSearchDirection() {
        if (this.currentX === 0 || this.mapWidth / this.currentX < 0.9) {
            this.searchPrimary = AntAction.MoveRight;
        } else {
            this.searchPrimary = AntAction.MoveLeft;
        }

        if (this.currentY === 0 || this.mapHeight / this.currentY < 0.9) {
            this.searchSecondary = AntAction.MoveDown;
        } else {
            this.searchSecondary = AntAction.MoveUp;
        }
    }

    private canMove(a: AntAction): boolean {
        let nextSpace: MapPosition;
        switch (a) {
            case AntAction.MoveRight:
                if (this.currentX + 1 === this.mapWidth) {
                    return false;
                }
                nextSpace = this.map[this.currentY][this.currentX + 1];
                break;
            case AntAction.MoveDown:
                if (this.currentY + 1 === this.mapHeight) {
                    return false;
                }
                nextSpace = this.map[this.currentY + 1][this.currentX];
                break;
            case AntAction.MoveLeft:
                if (this.currentX - 1 === -1) {
                    return false;
                }
                nextSpace = this.map[this.currentY][this.currentX - 1];
                break;
            case AntAction.MoveUp:
                if (this.currentY - 1 === -1) {
                    return false;
                }
                nextSpace = this.map[this.currentY - 1][this.currentX];
                break;
            default:
                return true;
        }
        return nextSpace.Known && !this.blocked(nextSpace.Item);

    }

    private blocked(i: Item): boolean {
        switch (i) {
            case Item.SteelWall:
            case Item.BrickWall:
            case Item.Bomb:
                return true;
            case Item.RedHome:
                return this.myColor !== ItemColor.Red;
            case Item.BlueHome:
                return this.myColor !== ItemColor.Blue;
            case Item.GreenHome:
                return this.myColor !== ItemColor.Green;
            case Item.OrangeHome:
                return this.myColor !== ItemColor.Orange;
            case Item.PinkHome:
                return this.myColor !== ItemColor.Pink;
            case Item.YellowHome:
                return this.myColor !== ItemColor.Yellow;
            case Item.GrayHome:
                return this.myColor !== ItemColor.Gray;
            case Item.WhiteHome:
                return this.myColor !== ItemColor.White;
            default:
                return false;
        }
    }

    private oppositeDirection(a: AntAction): AntAction {
        switch (a) {
            case AntAction.MoveRight:
                return AntAction.MoveLeft;
            case AntAction.MoveDown:
                return AntAction.MoveUp;
            case AntAction.MoveLeft:
                return AntAction.MoveRight;
            case AntAction.MoveUp:
                return AntAction.MoveDown;
            case AntAction.EchoRight:
                return AntAction.EchoLeft;
            case AntAction.EchoDown:
                return AntAction.EchoUp;
            case AntAction.EchoLeft:
                return AntAction.EchoRight;
            case AntAction.EchoUp:
                return AntAction.EchoDown;
            case AntAction.ShootRight:
                return AntAction.ShootLeft;
            case AntAction.ShootDown:
                return AntAction.ShootUp;
            case AntAction.ShootLeft:
                return AntAction.ShootRight;
            case AntAction.ShootUp:
                return AntAction.ShootDown;
            default:
                return AntAction.Wait;
        }
    }
}