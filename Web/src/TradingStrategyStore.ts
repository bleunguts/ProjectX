import { RootStore } from "./RootStore";
import { BackenedApi } from "./api";
import { reaction, runInAction, makeAutoObservable } from 'mobx';
import { FakeStrategyBTCUSD as FakeStrategyBTCUSData, FakeStrategyChartData, FakeStrategyIBMData, FakeStrategyPlaceholder } from "./components/layout/DummyData";
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

    loadErrorPlaceholder = () => {
        this.symbol = `ERROR LOADING SYMBOL..`;
        this.data = FakeStrategyPlaceholder;
    };

    loadChartData(symbol: string) {
        try {
            this.isLoading = true;
            runInAction(() => {
                this.transport        
                    .fetchLongShortStrategy(symbol)
                    .then((res) => {
                        const resp = (res as AxiosResponse<never, never>);
                        switch(resp.status) {
                            case 204: {
                                const cache = cachedData(symbol);
                                if(cache != undefined){
                                    this.data = cache as ChartData[];
                                } else {
                                    this.loadErrorPlaceholder();
                                }
                                break;
                            }
                            case 500: {
                                this.loadErrorPlaceholder();
                                break;
                            }
                            default:  {
                                this.data = (resp.data as ChartData[]);
                                break;
                            }
                        }
                    })
                    .catch((e) => {
                        console.log(`Error occurred whilst loading new symbol... ${e}`);
                        console.log(e);
                        this.loadErrorPlaceholder();
                    });
                
                console.log(`Symbol: ${symbol} loaded, data length: ${this.data.length}`);
            })
         }
         catch(e) {
            console.log(`Error occurred whilst loading new symbol... ${e}`);
            this.loadErrorPlaceholder();
         }
         finally {
            this.isLoading = false;
         }
    }
}

function cachedData(symbol: string): ChartData[] | undefined  {
    switch(symbol)
    {
        case 'AAPL': return FakeStrategyChartData;
        case 'IBM' : return FakeStrategyIBMData;
        case 'BTCUSD' : return FakeStrategyBTCUSData;
        default: return undefined;
    }
}

