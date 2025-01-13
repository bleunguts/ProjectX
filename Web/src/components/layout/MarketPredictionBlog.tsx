import * as React from "react";
import MainFeaturedPost from "./MainFeaturedPost";
import { Grid } from "@mui/material";
import MarketPrediction from "./MarketPrediction";

const mainFeaturedPost = {
    title: 'Predicting the future with Artiifical Intelligence',
    description:
          "Using the power of machine learning classification and regression models such as K-Nearest Neighbours, Support Vector Machines and Artificial Neural Networks we can teach machines how to think and predict future prices.",
    image: 'https://source.unsplash.com/random?wallpapers',
    imageText: 'main image description',  
    linkText: '',
  };

const bodyComponents = [
    'Training ML models for price predictions.',
];

export default function MarketPredictionBlog() {
    return (
        <div>
             <MainFeaturedPost post={mainFeaturedPost} />
             <Grid container spacing={5} sx={{ mt: 3 }}>
                <MarketPrediction title="Price Predictions" posts={bodyComponents} />           
            </Grid>
        </div>      
    );
}