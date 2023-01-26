namespace Module.Unity.Custermization
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    public enum PartAssetType
    {
        Renderer_MeshOrSkin,
        GameObject,
        Color,
        Material
    }

    public struct PartAssetData
    {
        private int partIndex;
        private bool sameBoneOrder;
        private PartAssetType assetType;
        private Renderer renderer;
        private GameObject gameObject;
        private Color? color;
        private Material material;
        private System.EventArgs args;

        public int PartIndex => partIndex;
        public bool SameBoneOrder => sameBoneOrder;
        public PartAssetType AssetType => assetType;
        public Renderer Renderer => renderer;
        public GameObject GameObject => gameObject;
        public Color? Color => color;
        public Material Material => material;
        public System.EventArgs Args => args;

        internal PartAssetData(int partIndex, MeshRenderer renderer, bool sameBoneOrder, EventArgs args = null)
        {
            this.partIndex = partIndex;
            this.renderer = renderer;
            this.sameBoneOrder = sameBoneOrder;

            assetType = PartAssetType.Renderer_MeshOrSkin;

            gameObject = null;
            color = null;
            material = null;
            this.args = args;
        }

        internal PartAssetData(int partIndex, SkinnedMeshRenderer renderer, bool sameBoneOrder, EventArgs args = null)
        {
            this.partIndex = partIndex;
            this.renderer = renderer;
            this.sameBoneOrder = sameBoneOrder;

            assetType = PartAssetType.Renderer_MeshOrSkin;

            gameObject = null;
            color = null;
            material = null;
            this.args = args;
        }

        internal PartAssetData(int partIndex, GameObject gameObject, EventArgs args = null)
        {
            this.partIndex = partIndex;

            renderer = null;
            sameBoneOrder = false;

            assetType = PartAssetType.GameObject;

            this.gameObject = gameObject;
            color = null;
            material = null;
            this.args = args;
        }

        internal PartAssetData(int partIndex, Color color, EventArgs args = null)
        {
            this.partIndex = partIndex;

            renderer = null;
            sameBoneOrder = false;

            assetType = PartAssetType.Color;

            gameObject = null;
            this.color = color;
            material = null;
            this.args = args;
        }


        internal PartAssetData(int partIndex, Material material, EventArgs args = null)
        {
            this.partIndex = partIndex;

            renderer = null;
            sameBoneOrder = false;

            assetType = PartAssetType.Material;

            gameObject = null;
            color = null;
            this.material = material;
            this.args = args;
        }

        public static PartAssetData Create(int partIdx, MeshRenderer renderer, EventArgs args = null)
        {
            return new PartAssetData(partIdx, renderer, true, args);
        }

        public static PartAssetData Create(int partIdx, MeshRenderer renderer, bool sameBoneOrder, EventArgs args = null)
        {
            return new PartAssetData(partIdx, renderer, sameBoneOrder, args);
        }

        public static PartAssetData Create(int partIdx, SkinnedMeshRenderer renderer, EventArgs args = null)
        {
            return new PartAssetData(partIdx, renderer, true, args);
        }

        public static PartAssetData Create(int partIdx, SkinnedMeshRenderer renderer, bool sameBoneOrder, EventArgs args = null)
        {
            return new PartAssetData(partIdx, renderer, sameBoneOrder, args);
        }

        public static PartAssetData Create(int partIdx, GameObject gameobject, EventArgs args = null)
        {
            return new PartAssetData(partIdx, gameobject, args);
        }
        public static PartAssetData Create(int partIdx, Color color, EventArgs args = null)
        {
            return new PartAssetData(partIdx, color, args);
        }

        public static PartAssetData Create(int partIdx, Material material, EventArgs args = null)
        {
            return new PartAssetData(partIdx, material, args);
        }

    }
}