import { RootStore } from "./RootStore";
import { BackenedApi } from "./api";
import { reaction, runInAction, makeAutoObservable } from 'mobx';
import { FakeStrategyPlaceholder } from "./components/layout/DummyData";
import { AxiosResponse } from "axios";

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
            runInAction(() => {
                this.transport        
                    .fetchLongShortStrategy(symbol)
                    .then((res) => this.data = ((res as AxiosResponse<never, never>).data as ChartData[]))
                    .catch((e) => {
                        console.log(`Error occurred whilst loading new symbol... ${e}`);
                        this.symbol = `ERROR LOADING SYMBOL..`;
                        this.data = FakeStrategyPlaceholder;
                    });
                
                console.log(`Symbol: ${symbol} loaded, data length: ${this.data.length}`);
            })
         }
         catch(e) {
            console.log(`Error occurred whilst loading new symbol... ${e}`);
            this.symbol = 'ERROR LOADING SYMBOL..';
            this.data = FakeStrategyPlaceholder;
         }
         finally {
            this.isLoading = false;
         }
    }
}

// function fetchData(symbol: string): ChartData[] {
//     switch(symbol)
//     {
//         case 'AAPL': return FakeStrategyChartData;
//         case 'IBM' : return FakeStrategyIBMData;
//         case 'BTCUSD' : return FakeStrategyBTCUSDData;
//         default: throw new Error(`Cannot load data for symbol ${symbol}`);
//     }
// }

