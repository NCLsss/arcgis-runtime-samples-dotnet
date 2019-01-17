﻿// Copyright 2019 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using System;
using Xamarin.Forms;

namespace ArcGISRuntimeXamarin.Samples.ManageOperationalLayers
{
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Manage operational layers",
        "Map",
        "Add, remove, and reorder operational layers in a map.",
        "")]
    public partial class ManageOperationalLayers : ContentPage
    {
        private MapViewModel _viewModel;

        // Some URLs of layers to add to the map.
        private readonly string[] _layerUrls = new[]
        {
            "http://sampleserver5.arcgisonline.com/arcgis/rest/services/Elevation/WorldElevations/MapServer",
            "http://sampleserver5.arcgisonline.com/arcgis/rest/services/Census/MapServer",
            "http://sampleserver5.arcgisonline.com/arcgis/rest/services/DamageAssessment/MapServer"
        };

        public ManageOperationalLayers()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            // Set up the view model and bindings.
            _viewModel = new MapViewModel(new Map(Basemap.CreateStreets()));
            MyMapView.Map = _viewModel.Map;
            IncludedListView.ItemsSource = _viewModel.IncludedLayers;
            ExcludedListView.ItemsSource = _viewModel.ExcludedLayers;

            // Add the layers.
            foreach (string layerUrl in _layerUrls)
            {
                _viewModel.AddLayerFromUrl(layerUrl);
            }
        }

        private void MoveButton_OnClicked(object sender, EventArgs e)
        {
            // Get the clicked button.
            Button clickedButton = (Button) sender;

            // Get the clicked layer.
            Layer clickedLayer = (Layer) clickedButton.BindingContext;

            // Move the layer.
            _viewModel.MoveLayer(clickedLayer);
        }

        private void DemoteButton_Clicked(object sender, EventArgs e)
        {
            // Get the clicked button.
            Button clickedButton = (Button) sender;

            // Get the clicked layer.
            Layer clickedLayer = (Layer) clickedButton.BindingContext;

            // Move the layer.
            _viewModel.DemoteLayer(clickedLayer);
        }

        private void PromoteButton_Clicked(object sender, EventArgs e)
        {
            // Get the clicked button.
            Button clickedButton = (Button) sender;

            // Get the clicked layer.
            Layer clickedLayer = (Layer) clickedButton.BindingContext;

            // Move the layer.
            _viewModel.PromoteLayer(clickedLayer);
        }
    }

    class MapViewModel
    {
        public Map Map { get; }
        public LayerCollection IncludedLayers => Map.OperationalLayers;
        public LayerCollection ExcludedLayers { get; } = new LayerCollection();

        public MapViewModel(Map map)
        {
            Map = map;
        }

        public void AddLayerFromUrl(string layerUrl)
        {
            ArcGISMapImageLayer layer = new ArcGISMapImageLayer(new Uri(layerUrl));
            Map.OperationalLayers.Add(layer);
        }

        public void DemoteLayer(Layer selectedLayer)
        {
            // Find the collection the layer is in.
            LayerCollection owningCollection = IncludedLayers.Contains(selectedLayer) ? IncludedLayers : ExcludedLayers;

            // Get the current index (position) of the layer.
            int layerIndex = owningCollection.IndexOf(selectedLayer);

            if (layerIndex == owningCollection.Count - 1)
            {
                return;
            }

            // Move the layer.
            owningCollection.Remove(selectedLayer);
            owningCollection.Insert(layerIndex + 1, selectedLayer);
        }

        public void PromoteLayer(Layer selectedLayer)
        {
            // Find the collection the layer is in.
            LayerCollection owningCollection = IncludedLayers.Contains(selectedLayer) ? IncludedLayers : ExcludedLayers;

            // Get the current index (position) of the layer.
            int layerIndex = owningCollection.IndexOf(selectedLayer);

            if (layerIndex < 1)
            {
                return;
            }

            // Move the layer.
            owningCollection.Remove(selectedLayer);
            owningCollection.Insert(layerIndex - 1, selectedLayer);
        }

        public void MoveLayer(Layer selectedLayer)
        {
            // Move the layer from one list to another.
            if (IncludedLayers.Contains(selectedLayer))
            {
                IncludedLayers.Remove(selectedLayer);
                ExcludedLayers.Add(selectedLayer);
            }
            else
            {
                ExcludedLayers.Remove(selectedLayer);
                IncludedLayers.Add(selectedLayer);
            }
        }
    }
}