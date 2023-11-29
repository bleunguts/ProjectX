// @ts-nocheck
import React from "react";
import Plot from "react-plotly.js";
import { Data, Datum } from "plotly.js";

export interface Chart3DProps {
    zData: Datum[][],
    label: string,    
  }

export default function Chart3D(props: Chart3DProps) {    
    const layout = { 
        title: props.label,            
        autosize: false,
        width: 700,
        height: 500,        
    };
    
    const data: Data = [
        {
          z: props.zData,
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