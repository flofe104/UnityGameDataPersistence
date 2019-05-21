﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class AssetReference : IAssetReferencer, IAssetInitializer, IAssetRefHolder, ISerializable
{

    public AssetReference() { }

    protected AssetReference(SerializationInfo info, StreamingContext context)
    {
        assetName = (string) info.GetValue(nameof(assetName), typeof(string));
        relativePathFromResource = (string) info.GetValue(nameof(relativePathFromResource), typeof(string));
        assetExtension = (string) info.GetValue(nameof(assetExtension), typeof(string));
        RestoreObject();
    }
    
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        IAssetReferencer referencer = GetSaveableReferencer();
        assetName = referencer?.AssetName;
        relativePathFromResource = referencer?.RelativePathFromResource;
        assetExtension = referencer?.AssetExtension;
        info.AddValue(nameof(assetName), assetName);
        info.AddValue(nameof(relativePathFromResource), relativePathFromResource);
        info.AddValue(nameof(assetExtension), assetExtension);
    }

    public abstract void InitializeAsset(Object assetRef);
    
    public abstract IAssetReferencer GetReferencer();

    protected abstract IAssetReferencer GetSaveableReferencer();
   
    public System.Type assetType;

    [SerializeField]
    private string relativePathFromResource;

    public string RelativePathFromResource
    {
        get { return relativePathFromResource; } set { relativePathFromResource = value; }
    }

    [SerializeField]
    private string assetName;

    public string AssetName
    {
        get { return assetName; }
        set { assetName = value; }
    }

    [SerializeField]
    private string assetExtension;

    public string AssetExtension
    {
        get { return assetExtension; }
        set { assetExtension = value; }
    }

    [SerializeField]
    private bool wasAlreadyValidated;

    public bool WasAlreadyValidated
    {
        get { return wasAlreadyValidated; }
        set { wasAlreadyValidated = value; }
    }

    protected abstract System.Type OjectType
    {
        get;
    }

    public Object RestoreObject()
    {
        Object result;
        if(!refRestoreTable.TryGetValue(this, out result))
        {
            result = Resources.Load(RelativePathFromResource + "/" + AssetName, OjectType);
            if (result != null)
            {
                refRestoreTable.Add(this, result);
            }
        }
        InitializeAsset(result);
        return result;
    }
    
    /// <summary>
    /// this dictionary will prevent the same asset references from 
    /// being searched in the resource folder multiple times
    /// </summary>
    private static Dictionary<AssetReference, Object> refRestoreTable = 
        new Dictionary<AssetReference, Object>();
    
}
