using TallyConnector.TDLReportSourceGenerator.Models;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Class;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class TDLReportTransformer
{
    private readonly IAssemblySymbol _assembly;
    private readonly List<ModelData> _symbolsToGenerate = [];
    private Dictionary<string, ModelData> _symbolsCache = [];

    public TDLReportTransformer(IAssemblySymbol assembly)
    {
        _assembly = assembly;
    }

    public async Task TransformAsync(INamedTypeSymbol symbol,
                                     CancellationToken token,
                                     HashSet<string>? uniqueComplexChilds = null)
    {
        ModelData modelData = await TransformModelDataAsync(symbol, uniqueComplexChilds, token: token);
        _symbolsToGenerate.Add(modelData);
    }

    private async Task<ModelData> TransformModelDataAsync(INamedTypeSymbol symbol,
                                                          HashSet<string>? uniqueComplexChilds,
                                                          string? collectionPrefix = null,
                                                          bool addMainSymbolCollection = false,
                                                          CancellationToken token = default)
    {
        var fullName = symbol.GetClassMetaName();
        _symbolsCache.TryGetValue(fullName, out var ExistingmodelData);
        if (ExistingmodelData != null)
        {
            return ExistingmodelData;
        }
        ModelData modelData = new(symbol);
        _symbolsCache[modelData.FullName] = modelData;
        uniqueComplexChilds ??= [];

        await TransformBaseSymbolDataAsync(modelData, symbol.BaseType, uniqueComplexChilds, collectionPrefix, token);

       // ClassAttributesTransformer.TransformAsync(modelData, symbol.GetAttributes());
        if (addMainSymbolCollection)
        {
            string? collectionName = modelData.TDLCollectionData?.CollectionName;
            if (collectionName is not null)
            {
                collectionPrefix = collectionPrefix is null ? collectionName : $"{collectionPrefix}.{collectionName}";
            }
        }
        await TransformMembers(modelData, symbol, uniqueComplexChilds, collectionPrefix, token: token);

        return modelData;
    }

    public List<ModelData> GetTransformedData()
    {
        return [.. _symbolsToGenerate];

    }


    private async Task TransformBaseSymbolDataAsync(ModelData modelData,
                                                    INamedTypeSymbol? baseType,
                                                    HashSet<string> complexProperties,
                                                    string? collectionPrefix = null,
                                                    CancellationToken token = default)
    {
        if (baseType == null || modelData.Symbol.TypeKind == TypeKind.Enum || baseType.HasFullyQualifiedMetadataName("object"))
        {
            return;
        }
        var fullName = baseType.GetClassMetaName();
        modelData.BaseData = new(baseType.GetClassMetaName(), baseType.Name);
        /* 
         * We shifted generate full attributes for same class instead of using base.GetFields(),
         * base.GetParts() because if any complex class is used in base class and current class it will include duplicate
         * fieds,lines, parts which will break tally request so we shifted once class generation approach
        */
        var isAlreadyIncluded = complexProperties.Contains(fullName);
        modelData.BaseData.Exclude = isAlreadyIncluded;
        complexProperties.Add(fullName);
        if (_symbolsCache.TryGetValue(fullName, out var ExistingmodelData))
        {
            modelData.BaseData.ModelData = ExistingmodelData;
            modelData.ComplexPropertiesCount += ExistingmodelData.ComplexPropertiesCount;
            //_modelData.SimplePropertiesCount += ExistingmodelData.SimplePropertiesCount;
            return;
        }
        var baseModelData = await TransformModelDataAsync(baseType, complexProperties, collectionPrefix, false, token);

        modelData.BaseData.ModelData = baseModelData;
        modelData.ComplexPropertiesCount += baseModelData.ComplexPropertiesCount;
        modelData.SimplePropertiesCount += baseModelData.SimplePropertiesCount;
        modelData.OveriddenComplexPropertiesCount += baseModelData.OveriddenComplexPropertiesCount;
        modelData.OveriddenSimplePropertiesCount += baseModelData.OveriddenSimplePropertiesCount;
        modelData.ENumPropertiesCount += baseModelData.ENumPropertiesCount;
        modelData.DefaultTDLFunctions.CopyFrom(baseModelData.DefaultTDLFunctions);
        modelData.TDLFunctions.CopyFrom(baseModelData.TDLFunctions);

        //await TransformMembers(_modelData, baseType, complexProperties, token: token);
        //await TransformBaseSymbolDataAsync(_modelData, baseType.BaseType, complexProperties, token);

    }


    private async Task TransformMembers(ModelData modelData,
                                        INamedTypeSymbol symbol,
                                        HashSet<string> complexProperties,
                                        string? collectionPrefix = null,
                                        CancellationToken token = default)
    {
        var members = symbol.GetMembers();
        foreach (var member in members)
        {
            if (member.DeclaredAccessibility != Accessibility.Public || member.Kind != SymbolKind.Property)
            {
                if (!modelData.IsEnum || member.Kind != SymbolKind.Field)
                {
                    continue;
                }
            }
            PropertyData propertyData = TransformMember(member, modelData);
            propertyData.SetFieldName($"{_assembly.Name}\0{modelData.FullName}");
            if (propertyData.IsComplex)
            {
                await TransformComplexProperySymbolDataAsync(modelData, propertyData, complexProperties, collectionPrefix, token);
            }
            else
            {
                modelData.SimplePropertiesCount++;
                if (propertyData.IsList)
                {
                    modelData.ComplexPropertiesCount++;
                }
                if (propertyData.IsEnum && !modelData.IsEnum)
                {
                    await TransformEnumSymbolData(modelData, propertyData, complexProperties, token);
                }
            }

            if (GetOverridenProperty(modelData, propertyData.Name, out PropertyData overridenProp))
            {
                overridenProp.IsOveridden = true;
                propertyData.OveriddenProperty = overridenProp;
                if (overridenProp.IsComplex)
                {
                    modelData.ComplexPropertiesCount--;
                    modelData.ComplexPropertiesCount -= overridenProp.OriginalModelData?.ComplexPropertiesCount ?? 0;
                    //_modelData.ComplexPropertiesCount -= overridenProp.OriginalModelData?.OveriddenComplexPropertiesCount ?? 0;
                    //_modelData.SimplePropertiesCount -= overridenProp.OriginalModelData?.SimplePropertiesCount ?? 0;
                    // _modelData.SimplePropertiesCount -= overridenProp.OriginalModelData?.OveriddenSimplePropertiesCount ?? 0;

                    modelData.OveriddenComplexPropertiesCount++;
                }
                else
                {
                    modelData.SimplePropertiesCount--;
                    modelData.OveriddenSimplePropertiesCount++;
                }
            }
            modelData.Properties[propertyData.Name] = propertyData;
        }
    }

    private static bool GetOverridenProperty(ModelData modelData, string name, out PropertyData overridenProp)
    {
        overridenProp = null;
        if (modelData.BaseData == null || modelData.BaseData.ModelData == null)
        {
            return false;
        }
        bool v = modelData.BaseData.ModelData.Properties.TryGetValue(name, out overridenProp);
        if (!v)
        {
            v = GetOverridenProperty(modelData.BaseData.ModelData, name, out overridenProp);
        }
        return v;
    }

    private async Task TransformEnumSymbolData(ModelData modelData,
                                               PropertyData propertyData,
                                               HashSet<string> complexProperties,
                                               CancellationToken token)
    {
        var propertyType = propertyData.PropertyOriginalType;
        var fullName = propertyType.GetClassMetaName();
        var isAlreadyIncluded = complexProperties.Contains(fullName);
        propertyData.Exclude = isAlreadyIncluded;
        complexProperties.Add(fullName);

        if (_symbolsCache.TryGetValue(fullName, out ModelData ComplexPropertyModelData))
        {

        }
        else
        {

            ComplexPropertyModelData = await TransformModelDataAsync(propertyType,
                                                                     complexProperties,
                                                                     propertyData.CollectionPrefix,
                                                                     false,
                                                                     token);
            modelData.ENumPropertiesCount++;
            modelData.ENumPropertiesCount += ComplexPropertyModelData.ENumPropertiesCount;
        }
        propertyData.OriginalModelData = ComplexPropertyModelData;
    }

    private async Task TransformComplexProperySymbolDataAsync(ModelData modelData,
                                                              PropertyData propertyData,
                                                              HashSet<string> complexProperties,
                                                              string? collectionPrefix = null,
                                                              CancellationToken token = default)
    {
        var propertyType = propertyData.PropertyOriginalType;
        var fullName = propertyType.GetClassMetaName();
        /* 
         * We shifted generate full attributes for same class instead of using ComplexClass.GetFields(),
         * v.GetParts() because if any complex class is used as property in  current class and complex class
         * it will include duplicate fieds,lines, parts 
         * which will break tally request so we shifted once class generation approach            
        */
        var isAlreadyIncluded = complexProperties.Contains(fullName);
        propertyData.Exclude = isAlreadyIncluded;
        modelData.ComplexPropertiesCount++;

        var isCollectionNameNull = propertyData.TDLCollectionData?.CollectionName is null;
        bool AddMainSymbolCollection = isCollectionNameNull;
        if (collectionPrefix is null)
        {
            propertyData.CollectionPrefix = propertyData.TDLCollectionData?.CollectionName;
        }
        else
        {
            if (!isCollectionNameNull)
            {
                propertyData.CollectionPrefix = $"{collectionPrefix}.{propertyData.TDLCollectionData!.CollectionName}";
            }
        }
        complexProperties.Add(fullName);
        if (_symbolsCache.TryGetValue(fullName, out ModelData ComplexPropertyModelData))
        {
            modelData.ComplexPropertiesCount += ComplexPropertyModelData.ComplexPropertiesCount;
            //_modelData.SimplePropertiesCount += ExistingmodelData.SimplePropertiesCount;
        }
        else
        {
            ComplexPropertyModelData = await TransformModelDataAsync(propertyType,
                                                                     complexProperties,
                                                                     propertyData.CollectionPrefix,
                                                                     AddMainSymbolCollection,
                                                                     token);

            modelData.ComplexPropertiesCount += ComplexPropertyModelData.ComplexPropertiesCount;
            modelData.SimplePropertiesCount += ComplexPropertyModelData.SimplePropertiesCount;
            modelData.ENumPropertiesCount += ComplexPropertyModelData.ENumPropertiesCount;
            modelData.DefaultTDLFunctions.CopyFrom(ComplexPropertyModelData.DefaultTDLFunctions);
            modelData.TDLFunctions.CopyFrom(ComplexPropertyModelData.TDLFunctions);

        }
        //await TransformMembers(ComplexPropertyModelData, propertyType, complexProperties, propertyData.CollectionPrefix, token);

        propertyData.OriginalModelData = ComplexPropertyModelData;

        propertyData.TDLCollectionData ??= ComplexPropertyModelData.TDLCollectionData;
        if (AddMainSymbolCollection)
        {
            string? collectionName = propertyData.TDLCollectionData?.CollectionName;
            if (collectionName is not null)
            {
                propertyData.CollectionPrefix = collectionPrefix is null ? collectionName : $"{collectionPrefix}.{collectionName}";
            }
        }


        foreach (var xMLData in propertyData.XMLData)
        {
            if (xMLData.Symbol == null)
            {
                continue;
            }
            if (_symbolsCache.TryGetValue(xMLData.Symbol.GetClassMetaName(), out ModelData xmlModelData))
            {
                modelData.ComplexPropertiesCount += xmlModelData.ComplexPropertiesCount;
                //_modelData.SimplePropertiesCount += ExistingmodelData.SimplePropertiesCount;
            }
            else
            {
                xmlModelData = await TransformModelDataAsync(xMLData.Symbol, complexProperties, propertyData.CollectionPrefix, true, token);
                modelData.ComplexPropertiesCount += xmlModelData.ComplexPropertiesCount;
                modelData.SimplePropertiesCount += xmlModelData.SimplePropertiesCount;
            }
            xMLData.ModelData = xmlModelData;
            if (xMLData.ModelData.TDLCollectionData?.CollectionName == null)
            {
                xMLData.CollectionPrefix = collectionPrefix;
            }
            else
            {
                xMLData.CollectionPrefix = collectionPrefix is null ? xMLData.ModelData.TDLCollectionData.CollectionName : $"{collectionPrefix}.{xMLData.ModelData.TDLCollectionData.CollectionName}";
            }
            // xMLData.CollectionPrefix = xMLData.ModelData.TDLCollectionData?.CollectionName;
            modelData.ComplexPropertiesCount++;

            xMLData.FieldName = $"{propertyData.Name}_{Utils.GenerateUniqueNameSuffix($"{propertyData.FieldName}\0{xMLData.Symbol.GetClassMetaName()}")}";

            //var fullName = xMLData.Symbol.GetClassMetaName();
            //_symbolsToGenerate.TryGetValue(fullName, out var ExistingmodelData);
            //if (ExistingmodelData != null)
            //{
            //    xMLData.ModelData = ExistingmodelData;
            //    xMLData.Exclude = true;
            //    continue;
            //}
            //xMLData.ModelData = new(xMLData.Symbol);
            //await TransformMembers(xMLData.ModelData, xMLData.Symbol, complexProperties, collectionPrefix, token);
            //if (!SymbolEqualityComparer.Default.Equals(xMLData.Symbol, propertyData.PropertyOriginalType))
            //{
            //    _modelData.ComplexPropertiesCount++;
            //}
        }


    }

    private PropertyData TransformMember(ISymbol member, ModelData modelData)
    {
        PropertyData propertyData = new(member, modelData);
       // PropertyAttributesTransformer.TransformAsync(propertyData, member.GetAttributes());
        if (propertyData.IsComplex || modelData.IsEnum || propertyData.PropertyOriginalType.TypeKind == TypeKind.Enum)
        {
            return propertyData;
        }

        switch (propertyData.PropertyOriginalType.SpecialType)
        {
            case SpecialType.System_Boolean:
                modelData.DefaultTDLFunctions.Add(GetBooleanFromLogicFieldMethodName);
                propertyData.TDLFieldData!.Set = $"$${GetBooleanFromLogicFieldFunctionName}:{propertyData.TDLFieldData!.Set}";
                break;
            case SpecialType.System_DateTime:
                modelData.DefaultTDLFunctions.Add(GetTransformDateToXSDMethodName);
                propertyData.TDLFieldData!.Set = $"$${GetTransformDateToXSDFunctionName}:{propertyData.TDLFieldData!.Set}";
                break;
            default:
                break;
        }
        return propertyData;
    }


    //private static int ExtractCountFromField(INamedTypeSymbol baseType, string fieldName)
    //    {
    //        var member = baseType.GetAllMembers(fieldName).FirstOrDefault();
    //        if (member is IFieldSymbol field)
    //        {
    //            return (int?)field.ConstantValue ?? 0;
    //        }
    //        return 0;
    //    }


    //private async Task TransformBaseSymbolDataAsync_Old(ModelData _modelData,
    //                                                INamedTypeSymbol? baseType,
    //                                                HashSet<string> uniqueComplexChilds,
    //                                                CancellationToken token)
    //{
    //    if (baseType == null || baseType.HasFullyQualifiedMetadataName("object"))
    //    {
    //        return;
    //    }
    //    bool isRequestableObjectInterface = baseType.HasInterfaceWithFullyQualifiedMetadataName(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName);

    //    var isMarkedPartial = baseType.DeclaringSyntaxReferences.Any(c => c.GetSyntax() is ClassDeclarationSyntax classDeclaration && classDeclaration.HasPartialKeyword());

    //    bool IsTallyRequestableObject = isRequestableObjectInterface || isMarkedPartial;
    //    _modelData.BaseData = new(baseType.GetClassMetaName(), baseType.Name)
    //    {
    //        // IsTallyRequestableObject = IsTallyRequestableObject
    //    };
    //    if (!IsTallyRequestableObject)
    //    {
    //        await TransformMembers(_modelData, baseType, uniqueComplexChilds, token: token);
    //        return;
    //    }
    //    bool isSameAssembly = SymbolEqualityComparer.Default.Equals(baseType.ContainingAssembly, _assembly);
    //    if (!isSameAssembly)
    //    {
    //        // Todo : Handle this when base class is from not same assembly we need to acess simple properties count and save
    //        //_modelData.SimplePropertiesCount = 2;
    //        _modelData.SimplePropertiesCount += ExtractCountFromField(baseType, SimpleFieldsCountFieldName);
    //        _modelData.ComplexPropertiesCount += ExtractCountFromField(baseType, ComplexFieldsCountFieldName);
    //        return;
    //    }
    //    ModelData baseModelData = await TransformAsync(baseType, token);
    //    baseModelData.IsRequestableObjectInterface = isRequestableObjectInterface;
    //    _modelData.BaseData.ModelData = baseModelData;
    //    _modelData.SimplePropertiesCount += _modelData.BaseData.ModelData.SimplePropertiesCount;
    //    _modelData.ComplexPropertiesCount += _modelData.BaseData.ModelData.ComplexPropertiesCount;

    //}
    //private async Task TransformComplexProperySymbolDataAsync_Old(ModelData _modelData,
    //                                               PropertyData propertyData,
    //                                               HashSet<string> complexProperties,
    //                                               CancellationToken token)
    //{
    //    var propertyType = propertyData.PropertyOriginalType;
    //    bool isRequestableObjectInterface = propertyType.HasInterfaceWithFullyQualifiedMetadataName(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName);

    //    var isMarkedPartial = propertyType.DeclaringSyntaxReferences.Any(c => c.GetSyntax() is ClassDeclarationSyntax classDeclaration && classDeclaration.HasPartialKeyword());

    //    bool IsTallyRequestableObject = isRequestableObjectInterface || isMarkedPartial;
    //    //spropertyData.IsTallyRequestableObject = IsTallyRequestableObject;
    //    if (!IsTallyRequestableObject)
    //    {
    //        await TransformMembers(_modelData, propertyType, complexProperties, token: token);
    //        return;
    //    }
    //    _modelData.ComplexPropertiesCount++;
    //    bool isSameAssembly = SymbolEqualityComparer.Default.Equals(propertyType.ContainingAssembly, _assembly);
    //    if (!isSameAssembly)
    //    {
    //        // Todo : Handle this when  class is from not same assembly we need to acess  properties count and save
    //        //_modelData.SimplePropertiesCount = 2;
    //        _modelData.SimplePropertiesCount += ExtractCountFromField(propertyType, SimpleFieldsCountFieldName);
    //        _modelData.ComplexPropertiesCount += ExtractCountFromField(propertyType, ComplexFieldsCountFieldName);
    //        return;
    //    }
    //    ModelData propertyModelData = await TransformAsync(propertyType, token);
    //    _modelData.SimplePropertiesCount += propertyModelData.SimplePropertiesCount;
    //    _modelData.ComplexPropertiesCount += propertyModelData.ComplexPropertiesCount;
    //    propertyData.OriginalModelData = propertyModelData;
    //}

}



