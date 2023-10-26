import { Datum } from "plotly.js";
import React from "react";
import Plot from "react-plotly.js";
import { FakeVolatilityData } from "./DummyData";

export interface Chart3DProps {
    data : unknown[],
    label: string,    
  }

export default function Chart3D(props: Chart3DProps) {    
    const layout = { 
        title: props.label,            
        autosize: false,
        width: 700,
        height: 500,        
    };
    
    const data = [
        {
          z: FakeVolatilityData,
          type: 'surface'     
        },
      ];
      
    const config = {
        displayModeBar: false
    };

    return (
        <React.Fragment>        
        <Plot data={data} layout={layout} config={config} />;
      </React.Fragment>
    );
}