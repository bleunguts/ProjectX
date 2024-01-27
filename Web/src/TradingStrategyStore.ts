import { RootStore } from "./RootStore";
import { BackenedApi } from "./api";
import { reaction, runInAction, makeAutoObservable } from 'mobx';
import { FakeStrategyBTCUSD as FakeStrategyBTCUSDData, FakeStrategyChartData, FakeStrategyIBMData } from "./components/layout/DummyData";

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
        try {
            this.isLoading = true;
            const data = fetchData(symbol);
            runInAction(() => {
                console.log(`Symbol: ${symbol} loaded, data length: ${data.length}`);
                this.data = data;
            })
         }
         catch(e) {
            console.log(`Error occurred whilst loading new symbol... ${e}`);
            this.symbol = 'ERROR LOADING SYMBOL..';
            this.data = FakeStrategyChartData;
         }
         finally {
            this.isLoading = false;
         }
    }
}

function fetchData(symbol: string): ChartData[] {
    switch(symbol)
    {
        case 'AAPL': return FakeStrategyChartData;
        case 'IBM' : return FakeStrategyIBMData;
        case 'BTCUSD' : return FakeStrategyBTCUSDData;
        default: throw new Error(`Cannot load data for symbol ${symbol}`);
    }
}

