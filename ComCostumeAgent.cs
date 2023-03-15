namespace Module.Unity.Custermization
{
    using Module.Core.Systems;
    using Module.Core.Systems.Events;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ComCostumeAgent : MonoBehaviour
    {
        [SerializeField] int id;
        [SerializeField] List<PartSlotInfo> slots;

        public int Id => id;
        public IReadOnlyList<PartSlotInfo> Slots => slots;

        public PartSlotInfo this[int partIndex]
        {
            get
            {
                UnityEngine.Assertions.Assert.IsTrue(partIndex >= 0 && partIndex < slots.Count, "part' index is incorrect");
                return slots[partIndex];
            }
        }

        public bool TryDetach(int partIndex, out PartAssetData? preAssetData)
        {
            UnityEngine.Assertions.Assert.IsTrue(partIndex >= 0 && partIndex < slots.Count, "part' index is incorrect");

            var slotInfo = slots[partIndex];

            var renderer = slotInfo.Slot.GetComponent<Renderer>();

            if (renderer == null)
            {
                PartAssetData? oldAssetData = slots[partIndex].AssetData;

                Transform child = null;

                if (slotInfo.Slot.childCount > 0)
                    child = slotInfo.Slot.GetChild(0);

                if (child != null)
                    child.parent = null;

                preAssetData = oldAssetData;

                slotInfo.SetAssetData(null);

                return true;
            }
            else
            {
                preAssetData = null;
                return false;
            }
        }

        public void ChangeOrAttach(PartAssetData assetData, out PartAssetData? preAssetData)
        {
            UnityEngine.Assertions.Assert.IsTrue(assetData.PartIndex >= 0 && assetData.PartIndex < slots.Count, "part' index is incorrect");

            PartAssetData? oldAssetData = slots[assetData.PartIndex].AssetData;
            ChangeOrAttach(assetData);

            preAssetData = oldAssetData;
        }

        public void ChangeOrAttach(PartAssetData assetData)
        {
            UnityEngine.Assertions.Assert.IsTrue(assetData.PartIndex >= 0 && assetData.PartIndex < slots.Count, "part' index is incorrect");

            var slotInfo = slots[assetData.PartIndex];

            switch (assetData.AssetType)
            {
                case PartAssetType.Renderer_MeshOrSkin:
                    if (slotInfo.IsSkinnedMesh)
                    {
                        var srcRenderer = slotInfo.Renderer as SkinnedMeshRenderer;
                        var newRenderer = assetData.Renderer as SkinnedMeshRenderer;

                        srcRenderer.sharedMesh = newRenderer.sharedMesh;
                        srcRenderer.sharedMaterial = newRenderer.sharedMaterial;

                        if (!assetData.SameBoneOrder)
                        {
                            Transform[] childs = transform.GetComponentsInChildren<Transform>(true);

                            Transform[] bones = new Transform[newRenderer.bones.Length];

                            for (int boneOrder = 0, range = newRenderer.bones.Length; boneOrder < range; ++boneOrder)
                            {
                                bones[boneOrder] = System.Array.Find<Transform>(childs, c => c.name == newRenderer.bones[boneOrder].name);
                            }

                            srcRenderer.bones = bones;
                        }
                    }
                    else
                    {
                        var srcRenderer = slotInfo.Renderer as MeshRenderer;
                        var newRenderer = slotInfo.Renderer as MeshRenderer;

                        srcRenderer.GetComponent<MeshFilter>().sharedMesh = newRenderer.GetComponent<MeshFilter>().sharedMesh;
                        srcRenderer.sharedMaterial = newRenderer.sharedMaterial;
                    }
                    break;

                case PartAssetType.GameObject:
                    Transform child = null;

                    if (slotInfo.Slot.childCount > 0)
                        child = slotInfo.Slot.GetChild(0);

                    if (child != null)
                        child.parent = null;

                    if (assetData.GameObject != null)
                    {
                        assetData.GameObject.transform.parent = slotInfo.Slot;
                        assetData.GameObject.transform.localPosition = Vector3.zero;
                        assetData.GameObject.transform.localRotation = Quaternion.identity;
                        assetData.GameObject.transform.localScale = Vector3.one;
                    }
                    break;
                case PartAssetType.Color:
                    if(assetData.Args == null)
                    {
                        slotInfo.Renderer.material.color = assetData.Color.Value;
                    }
                    else
                    {
                        EventArgs<int>? val = assetData.Args as EventArgs<int>?;
                        if (!val.HasValue)
                            return;
                        if (slotInfo.Renderer.materials.Length - 1 < val.Value.Arg1 || val.Value.Arg1 < 0)
                            return;
                        slotInfo.Renderer.materials[val.Value.Arg1].color = assetData.Color.Value;
                    }                   
                    break;
                case PartAssetType.Material:
                    if (assetData.Args == null)
                    {
                        slotInfo.Renderer.sharedMaterial = assetData.Material;
                    }
                    else
                    {
                        EventArgs<int>? val = assetData.Args as EventArgs<int>?;
                        if (!val.HasValue)
                            return;
                        if (slotInfo.Renderer.materials.Length - 1 < val.Value.Arg1 || val.Value.Arg1 < 0)
                            return;
                        slotInfo.Renderer.sharedMaterials[val.Value.Arg1] = assetData.Material;
                    }
                    break;
            }

            slotInfo.SetAssetData(assetData);
        }



#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            Vector3 before = meshRenderer.bones[0].position;
            for (int i = 0, range = meshRenderer.bones.Length; i < range; ++i)
            {
                Gizmos.DrawLine(meshRenderer.bones[i].position, before);
                UnityEditor.Handles.Label(meshRenderer.bones[i].transform.position, i.ToString());
                before= meshRenderer.bones[i].position;
            }
        }
#endif
    }
}
