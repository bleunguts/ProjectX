import { RootStore } from "./RootStore";
import { BackenedApi } from "./api";
import { makeAutoObservable, runInAction} from "mobx"
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

    constructor(root: RootStore, transport: BackenedApi) {
      makeAutoObservable(this);

      this.root = root
      this.transport = transport;

      this.loadChartData();
    }

    loadChartData() {
        this.isLoading = true;
        runInAction(() => {
            console.log(`Data length: ${FakeStrategyChartData.length}`);
            this.data = FakeStrategyChartData;
            this.isLoading = false;
        })
    }
}


  