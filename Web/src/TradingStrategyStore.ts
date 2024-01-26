import { RootStore } from "./RootStore";
import { BackenedApi } from "./api";
import { reaction, runInAction, makeAutoObservable, observable } from 'mobx';
import { FakeStrategyChartData } from "./components/layout/DummyData";

export interface ChartData {
    time: string,
    amount: number,
    amountHold: number
}

export class TradingStrategyStore
{
    root: RootStore;
    transport: BackenedApi;

    isLoading: boolean = false;
    data: ChartData[] = [];
    symbol : string;

    constructor(root: RootStore, transport: BackenedApi) {
      this.root = root
      this.transport = transport;
      makeAutoObservable(this);

      this.symbol = 'AAPL';
      this.loadChartData(this.symbol);

      reaction(
            () => this.symbol, 
            newSymbol => { this.loadChartData(newSymbol)}
      );
    }

    loadChartData(symbol: string) {
        this.isLoading = true;
        runInAction(() => {
            console.log(`Symbol: ${symbol} loaded, Data length: ${FakeStrategyChartData.length}`);
            this.data = FakeStrategyChartData;
            this.isLoading = false;
        })
    }
}
