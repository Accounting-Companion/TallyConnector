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
        _symbolsCache = [];
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

        uniqueComplexChilds ??= [];

        await TransformBaseSymbolDataAsync(modelData, symbol.BaseType, uniqueComplexChilds, collectionPrefix, token);

        ClassAttributesTransformer.TransformAsync(modelData, symbol.GetAttributes());
        if (addMainSymbolCollection)
        {
            string? collectionName = modelData.TDLCollectionData?.CollectionName;
            if (collectionName is not null)
            {
                collectionPrefix = collectionPrefix is null ? collectionName : $"{collectionPrefix}.{collectionName}";
            }
        }
        await TransformMembers(modelData, symbol, uniqueComplexChilds, collectionPrefix, token: token);

        _symbolsCache[modelData.FullName] = modelData;
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
            //modelData.SimplePropertiesCount += ExistingmodelData.SimplePropertiesCount;
            return;
        }
        var baseModelData = await TransformModelDataAsync(baseType, complexProperties, collectionPrefix, false, token);

        modelData.BaseData.ModelData = baseModelData;
        modelData.ComplexPropertiesCount += baseModelData.ComplexPropertiesCount;
        modelData.SimplePropertiesCount += baseModelData.SimplePropertiesCount;
        modelData.ENumPropertiesCount += baseModelData.ENumPropertiesCount;
        modelData.DefaultTDLFunctions.CopyFrom(baseModelData.DefaultTDLFunctions);
        modelData.TDLFunctions.CopyFrom(baseModelData.TDLFunctions);
        
        //await TransformMembers(modelData, baseType, complexProperties, token: token);
        //await TransformBaseSymbolDataAsync(modelData, baseType.BaseType, complexProperties, token);

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
            if (modelData.BaseData?.ModelData?.Properties.TryGetValue(propertyData.Name, out var overridenProp) ?? false)
            {
                overridenProp.IsOveridden = true;
                propertyData.OveriddenProperty = overridenProp;
                if (propertyData.IsComplex)
                {
                    modelData.ComplexPropertiesCount--;
                }
                else
                {
                    modelData.SimplePropertiesCount--;
                }
            }
            modelData.Properties[propertyData.Name] = propertyData;
        }
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
            //modelData.SimplePropertiesCount += ExistingmodelData.SimplePropertiesCount;
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
                //modelData.SimplePropertiesCount += ExistingmodelData.SimplePropertiesCount;
            }
            else
            {
                xmlModelData = await TransformModelDataAsync(xMLData.Symbol, complexProperties, collectionPrefix, true, token);
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
            xMLData.CollectionPrefix = xMLData.ModelData.TDLCollectionData?.CollectionName;
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
            //    modelData.ComplexPropertiesCount++;
            //}
        }


    }

    private PropertyData TransformMember(ISymbol member, ModelData modelData)
    {
        PropertyData propertyData = new(member, modelData);
        PropertyAttributesTransformer.TransformAsync(propertyData, member.GetAttributes());
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


    //private async Task TransformBaseSymbolDataAsync_Old(ModelData modelData,
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
    //    modelData.BaseData = new(baseType.GetClassMetaName(), baseType.Name)
    //    {
    //        // IsTallyRequestableObject = IsTallyRequestableObject
    //    };
    //    if (!IsTallyRequestableObject)
    //    {
    //        await TransformMembers(modelData, baseType, uniqueComplexChilds, token: token);
    //        return;
    //    }
    //    bool isSameAssembly = SymbolEqualityComparer.Default.Equals(baseType.ContainingAssembly, _assembly);
    //    if (!isSameAssembly)
    //    {
    //        // Todo : Handle this when base class is from not same assembly we need to acess simple properties count and save
    //        //modelData.SimplePropertiesCount = 2;
    //        modelData.SimplePropertiesCount += ExtractCountFromField(baseType, SimpleFieldsCountFieldName);
    //        modelData.ComplexPropertiesCount += ExtractCountFromField(baseType, ComplexFieldsCountFieldName);
    //        return;
    //    }
    //    ModelData baseModelData = await TransformAsync(baseType, token);
    //    baseModelData.IsRequestableObjectInterface = isRequestableObjectInterface;
    //    modelData.BaseData.ModelData = baseModelData;
    //    modelData.SimplePropertiesCount += modelData.BaseData.ModelData.SimplePropertiesCount;
    //    modelData.ComplexPropertiesCount += modelData.BaseData.ModelData.ComplexPropertiesCount;

    //}
    //private async Task TransformComplexProperySymbolDataAsync_Old(ModelData modelData,
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
    //        await TransformMembers(modelData, propertyType, complexProperties, token: token);
    //        return;
    //    }
    //    modelData.ComplexPropertiesCount++;
    //    bool isSameAssembly = SymbolEqualityComparer.Default.Equals(propertyType.ContainingAssembly, _assembly);
    //    if (!isSameAssembly)
    //    {
    //        // Todo : Handle this when  class is from not same assembly we need to acess  properties count and save
    //        //modelData.SimplePropertiesCount = 2;
    //        modelData.SimplePropertiesCount += ExtractCountFromField(propertyType, SimpleFieldsCountFieldName);
    //        modelData.ComplexPropertiesCount += ExtractCountFromField(propertyType, ComplexFieldsCountFieldName);
    //        return;
    //    }
    //    ModelData propertyModelData = await TransformAsync(propertyType, token);
    //    modelData.SimplePropertiesCount += propertyModelData.SimplePropertiesCount;
    //    modelData.ComplexPropertiesCount += propertyModelData.ComplexPropertiesCount;
    //    propertyData.OriginalModelData = propertyModelData;
    //}

}



