import { TradingStrategyStore } from "./TradingStrategyStore"
import api from "./api"

export class RootStore {
    tradingStrategyStore: TradingStrategyStore
  
    constructor() {
      this.tradingStrategyStore = new TradingStrategyStore(this, api);
    }
}

