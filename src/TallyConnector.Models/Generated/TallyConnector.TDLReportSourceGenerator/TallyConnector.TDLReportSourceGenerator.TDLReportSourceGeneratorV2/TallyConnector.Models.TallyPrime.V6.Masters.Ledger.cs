using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Models.TallyPrime.V6.Masters;
/*
* Generated based on TallyConnector.Models.TallyPrime.V6.Masters.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string GUID_QW2F_FieldName = "GUID_QW2F";
    const string RemoteId_4JMJ_FieldName = "RemoteId_4JMJ";
    const string MasterId_2JFP_FieldName = "MasterId_2JFP";
    const string AlterId_RUKQ_FieldName = "AlterId_RUKQ";
    const string EnteredBy_WTJ3_FieldName = "EnteredBy_WTJ3";
    const string AlteredBy_K5HW_FieldName = "AlteredBy_K5HW";
    const string Name_SMP5_FieldName = "Name_SMP5";
    const string Alias_ADB2_FieldName = "Alias_ADB2";
    const string Group_UWMB_FieldName = "Group_UWMB";
    const string Currency_TK7A_FieldName = "Currency_TK7A";
    const string TaxType_DQI4_FieldName = "TaxType_DQI4";
    const string GSTTaxType_DTP7_FieldName = "GSTTaxType_DTP7";
    const string RateofTax_QQSR_FieldName = "RateofTax_QQSR";
    const string AppropriateFor_M8GS_FieldName = "AppropriateFor_M8GS";
    const string IsBillWise_1X7Z_FieldName = "IsBillWise_1X7Z";
    const string NameList_ZIWC_FieldName = "NameList_ZIWC";
    const string LanguageId_XANV_FieldName = "LanguageId_XANV";
    const string Amount_C0LF_FieldName = "Amount_C0LF";
    const string Currency_J80N_FieldName = "Currency_J80N";
    const string ForexAmount_MT3L_FieldName = "ForexAmount_MT3L";
    const string ForexCurrency_LA75_FieldName = "ForexCurrency_LA75";
    const string RateOfExchange_QSXJ_FieldName = "RateOfExchange_QSXJ";
    const string IsDebit_SY5U_FieldName = "IsDebit_SY5U";
    const string AddressName_Z6JX_FieldName = "AddressName_Z6JX";
    const string AddressLines_CAF9_FieldName = "AddressLines_CAF9";
    const string Country_FRRJ_FieldName = "Country_FRRJ";
    const string State_CNG2_FieldName = "State_CNG2";
    const string PinCode_OO6T_FieldName = "PinCode_OO6T";
    const string ContactPerson_T0EI_FieldName = "ContactPerson_T0EI";
    const string MobileNo_2KZB_FieldName = "MobileNo_2KZB";
    const string PhoneNumber_KFKX_FieldName = "PhoneNumber_KFKX";
    const string FaxNumber_HQNP_FieldName = "FaxNumber_HQNP";
    const string Email_S2QE_FieldName = "Email_S2QE";
    const string PANNumber_1WOF_FieldName = "PANNumber_1WOF";
    const string VATNumber_D5F0_FieldName = "VATNumber_D5F0";
    const string CSTNumber_PWWU_FieldName = "CSTNumber_PWWU";
    const string ExciseNatureOfPurchase_UV2K_FieldName = "ExciseNatureOfPurchase_UV2K";
    const string ExciseRegistrationNo_LLVU_FieldName = "ExciseRegistrationNo_LLVU";
    const string ExciseImportRegistrationNo_5EMT_FieldName = "ExciseImportRegistrationNo_5EMT";
    const string ImportExportCode_2JP6_FieldName = "ImportExportCode_2JP6";
    const string GSTDealerType_MAMX_FieldName = "GSTDealerType_MAMX";
    const string IsOtherTerritoryAssessee_IKOQ_FieldName = "IsOtherTerritoryAssessee_IKOQ";
    const string GSTIN_7NPB_FieldName = "GSTIN_7NPB";
    const string ApplicableFrom_Q1D6_FieldName = "ApplicableFrom_Q1D6";
    const string Range_CZ2U_FieldName = "Range_CZ2U";
    const string Division_SL2I_FieldName = "Division_SL2I";
    const string Commissionerate_4I3E_FieldName = "Commissionerate_4I3E";
    const string AdressLines_C4G4_FieldName = "AdressLines_C4G4";
    const string ApplicableFrom_QLJQ_FieldName = "ApplicableFrom_QLJQ";
    const string MailingName_ZIWT_FieldName = "MailingName_ZIWT";
    const string Country_YDYN_FieldName = "Country_YDYN";
    const string State_FSSQ_FieldName = "State_FSSQ";
    const string PINCode_SFTL_FieldName = "PINCode_SFTL";
    const string ApplicableFrom_IJLE_FieldName = "ApplicableFrom_IJLE";
    const string GSTRegistrationType_KGBW_FieldName = "GSTRegistrationType_KGBW";
    const string State_PJCH_FieldName = "State_PJCH";
    const string PlaceOfSupply_QR0I_FieldName = "PlaceOfSupply_QR0I";
    const string IsOtherTerritoryAssesse_EZRV_FieldName = "IsOtherTerritoryAssesse_EZRV";
    const string ConsiderPurchaseForExport_DLDG_FieldName = "ConsiderPurchaseForExport_DLDG";
    const string IsTransporter_JL8F_FieldName = "IsTransporter_JL8F";
    const string TransporterId_MJQC_FieldName = "TransporterId_MJQC";
    const string IsCommonParty_YTBM_FieldName = "IsCommonParty_YTBM";
    const string GSTIN_JA0L_FieldName = "GSTIN_JA0L";
    const string LanguageNameList_S9R3_PartName = "LanguageNameList_S9R3";
    const string OpeningBalance_AZIU_PartName = "OpeningBalance_AZIU";
    const string Addresses_1S2P_PartName = "Addresses_1S2P";
    const string MailingDetails_E1QZ_PartName = "MailingDetails_E1QZ";
    const string GSTRegistrationDetails_JNXL_PartName = "GSTRegistrationDetails_JNXL";
    const string NameList_ZIWC_PartName = "NameList_ZIWC";
    const string AddressLines_CAF9_PartName = "AddressLines_CAF9";
    const string ExciseJurisdictions_PGDJ_PartName = "ExciseJurisdictions_PGDJ";
    const string AdressLines_C4G4_PartName = "AdressLines_C4G4";
    const string ReportName = "Ledger_BS7O";
    const string CollectionName = "LedgersCollection_BS7O";
    const string XMLTag = "LEDGER";
    const int SimpleFieldsCount = 63;
    const int ComplexFieldsCount = 9;
    public static global::TallyConnector.Core.Models.Request.RequestEnvelope GetRequestEnvelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.Request.RequestEnvelope(global::TallyConnector.Core.Models.Request.HType.Data, ReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(ReportName)];
        tdlMsg.Form = [new(ReportName)];
        tdlMsg.Part = [GetMainTDLPart(), ..GetTDLParts()];
        tdlMsg.Line = [GetMainTDLLine(), ..GetTDLLines()];
        tdlMsg.Field = [..GetTDLFields()];
        tdlMsg.Collection = [..GetTDLCollections()];
        return reqEnvelope;
    }

    public static global::System.Xml.Serialization.XmlAttributeOverrides GetXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        var XmlAttributes = new global::System.Xml.Serialization.XmlAttributes();
        XmlAttributes.XmlElements.Add(new(XMLTag));
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::TallyConnector.Models.TallyPrime.V6.Masters.Ledger>.TypeInfo, "Objects", XmlAttributes);
        return xmlAttributeOverrides;
    }

    public static global::TallyConnector.Core.Models.Request.Part GetMainTDLPart(string partName = ReportName, string? collectionName = CollectionName, string? xmlTag = null)
    {
        return new(partName, collectionName, partName)
        {
            XMLTag = xmlTag
        };
    }

    public static global::TallyConnector.Core.Models.Request.Part[] GetTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Request.Part[ComplexFieldsCount];
        parts[0] = new(LanguageNameList_S9R3_PartName, "LanguageName");
        parts[1] = new(OpeningBalance_AZIU_PartName, "OpeningBalance");
        parts[2] = new(Addresses_1S2P_PartName, "LEDMULTIADDRESSLIST");
        parts[3] = new(MailingDetails_E1QZ_PartName, "LEDMAILINGDETAILS");
        parts[4] = new(GSTRegistrationDetails_JNXL_PartName, "LEDGSTREGDETAILS");
        parts[5] = new(NameList_ZIWC_PartName, "Name")
        {
            XMLTag = "NAME.LIST"
        };
        parts[6] = new(AddressLines_CAF9_PartName, "ADDRESS")
        {
            XMLTag = "ADDRESS.LIST"
        };
        parts[7] = new(ExciseJurisdictions_PGDJ_PartName, "EXCISEJURISDICTIONDETAILS");
        parts[8] = new(AdressLines_C4G4_PartName, "ADDRESS")
        {
            XMLTag = "ADDRESS.LIST"
        };
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [GUID_QW2F_FieldName,RemoteId_4JMJ_FieldName,MasterId_2JFP_FieldName,AlterId_RUKQ_FieldName,EnteredBy_WTJ3_FieldName,AlteredBy_K5HW_FieldName,Name_SMP5_FieldName,Alias_ADB2_FieldName,Group_UWMB_FieldName,Currency_TK7A_FieldName,TaxType_DQI4_FieldName,GSTTaxType_DTP7_FieldName,RateofTax_QQSR_FieldName,AppropriateFor_M8GS_FieldName,IsBillWise_1X7Z_FieldName], XMLTag)
        {
            Explode = [$"{LanguageNameList_S9R3_PartName}:YES", $"{OpeningBalance_AZIU_PartName}:{string.Format("NOT $$IsEmpty:{0}", "OpeningBalance")}", $"{Addresses_1S2P_PartName}:{string.Format("$$NUMITEMS:LEDMULTIADDRESSLIST>0", "Addresses")}", $"{MailingDetails_E1QZ_PartName}:{string.Format("$$NUMITEMS:LEDMAILINGDETAILS>0", "MailingDetails")}", $"{GSTRegistrationDetails_JNXL_PartName}:{string.Format("$$NUMITEMS:LEDGSTREGDETAILS>0", "GSTRegistrationDetails")}"]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(LanguageNameList_S9R3_PartName, [LanguageId_XANV_FieldName], "LANGUAGENAME.LIST")
        {
            Explode = [$"{NameList_ZIWC_PartName}:YES"]
        };
        _lines[1] = new(OpeningBalance_AZIU_PartName, [Amount_C0LF_FieldName,Currency_J80N_FieldName,ForexAmount_MT3L_FieldName,ForexCurrency_LA75_FieldName,RateOfExchange_QSXJ_FieldName,IsDebit_SY5U_FieldName], "OPENINGBALANCE");
        _lines[2] = new(Addresses_1S2P_PartName, [AddressName_Z6JX_FieldName,Country_FRRJ_FieldName,State_CNG2_FieldName,PinCode_OO6T_FieldName,ContactPerson_T0EI_FieldName,MobileNo_2KZB_FieldName,PhoneNumber_KFKX_FieldName,FaxNumber_HQNP_FieldName,Email_S2QE_FieldName,PANNumber_1WOF_FieldName,VATNumber_D5F0_FieldName,CSTNumber_PWWU_FieldName,ExciseNatureOfPurchase_UV2K_FieldName,ExciseRegistrationNo_LLVU_FieldName,ExciseImportRegistrationNo_5EMT_FieldName,ImportExportCode_2JP6_FieldName,GSTDealerType_MAMX_FieldName,IsOtherTerritoryAssessee_IKOQ_FieldName,GSTIN_7NPB_FieldName], "LEDMULTIADDRESSLIST.LIST")
        {
            Explode = [$"{AddressLines_CAF9_PartName}:YES", $"{ExciseJurisdictions_PGDJ_PartName}:{string.Format("$$NUMITEMS:EXCISEJURISDICTIONDETAILS>0", "ExciseJurisdictions")}"]
        };
        _lines[3] = new(MailingDetails_E1QZ_PartName, [ApplicableFrom_QLJQ_FieldName,MailingName_ZIWT_FieldName,Country_YDYN_FieldName,State_FSSQ_FieldName,PINCode_SFTL_FieldName], "LEDMAILINGDETAILS.LIST")
        {
            Explode = [$"{AdressLines_C4G4_PartName}:YES"]
        };
        _lines[4] = new(GSTRegistrationDetails_JNXL_PartName, [ApplicableFrom_IJLE_FieldName,GSTRegistrationType_KGBW_FieldName,State_PJCH_FieldName,PlaceOfSupply_QR0I_FieldName,IsOtherTerritoryAssesse_EZRV_FieldName,ConsiderPurchaseForExport_DLDG_FieldName,IsTransporter_JL8F_FieldName,TransporterId_MJQC_FieldName,IsCommonParty_YTBM_FieldName,GSTIN_JA0L_FieldName], "LEDGSTREGDETAILS.LIST");
        _lines[5] = new(NameList_ZIWC_PartName, [NameList_ZIWC_FieldName]);
        _lines[6] = new(AddressLines_CAF9_PartName, [AddressLines_CAF9_FieldName]);
        _lines[7] = new(ExciseJurisdictions_PGDJ_PartName, [ApplicableFrom_Q1D6_FieldName,Range_CZ2U_FieldName,Division_SL2I_FieldName,Commissionerate_4I3E_FieldName], "EXCISEJURISDICTIONDETAILS.LIST");
        _lines[8] = new(AdressLines_C4G4_PartName, [AdressLines_C4G4_FieldName]);
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(GUID_QW2F_FieldName, "GUID", "$GUID");
        _fields[1] = new(RemoteId_4JMJ_FieldName, "REMOTEALTGUID", "$REMOTEALTGUID");
        _fields[2] = new(MasterId_2JFP_FieldName, "MASTERID", "$MASTERID");
        _fields[3] = new(AlterId_RUKQ_FieldName, "ALTERID", "$ALTERID");
        _fields[4] = new(EnteredBy_WTJ3_FieldName, "ENTEREDBY", "$ENTEREDBY");
        _fields[5] = new(AlteredBy_K5HW_FieldName, "ALTEREDBY", "$ALTEREDBY");
        _fields[6] = new(Name_SMP5_FieldName, "NAME", "$NAME");
        _fields[7] = new(Alias_ADB2_FieldName, "ALIAS", "$_FirstAlias");
        _fields[8] = new(Group_UWMB_FieldName, "PARENT", "$PARENT");
        _fields[9] = new(Currency_TK7A_FieldName, "CURRENCY", "$CURRENCY");
        _fields[10] = new(TaxType_DQI4_FieldName, "TAXTYPE", "$TAXTYPE");
        _fields[11] = new(GSTTaxType_DTP7_FieldName, "GSTTYPE", "$GSTTYPE");
        _fields[12] = new(RateofTax_QQSR_FieldName, "RATEOFTAXCALCULATION", "$RATEOFTAXCALCULATION");
        _fields[13] = new(AppropriateFor_M8GS_FieldName, "APPROPRIATEFOR", "$APPROPRIATEFOR");
        _fields[14] = new(IsBillWise_1X7Z_FieldName, "ISBILLWISEON", "$ISBILLWISEON");
        _fields[15] = new(NameList_ZIWC_FieldName, "NAME", "$NAME");
        _fields[16] = new(LanguageId_XANV_FieldName, "LANGUAGEID", "$LANGUAGEID");
        _fields[17] = new(Amount_C0LF_FieldName, "AMOUNT", "$$BaseValue:{0}");
        _fields[18] = new(Currency_J80N_FieldName, "CURRENCY", "$CurrencyName:Company:##SVCurrentCompany");
        _fields[19] = new(ForexAmount_MT3L_FieldName, "FOREXAMOUNT", "$$ForexValue:{0}");
        _fields[20] = new(ForexCurrency_LA75_FieldName, "FOREXSYMBOL", "$FOREXSYMBOL");
        _fields[21] = new(RateOfExchange_QSXJ_FieldName, "RATEOFEXCHANGE", "$$RatexValue:{0}");
        _fields[22] = new(IsDebit_SY5U_FieldName, "ISDEBIT", "$$IsDebit:{0}");
        _fields[23] = new(AddressName_Z6JX_FieldName, "MAILINGNAME", "$MAILINGNAME");
        _fields[24] = new(AddressLines_CAF9_FieldName, "ADDRESS", "$ADDRESS");
        _fields[25] = new(Country_FRRJ_FieldName, "COUNTRYNAME", "$COUNTRYNAME");
        _fields[26] = new(State_CNG2_FieldName, "LEDSTATENAME", "$LEDSTATENAME");
        _fields[27] = new(PinCode_OO6T_FieldName, "PINCODE", "$PINCODE");
        _fields[28] = new(ContactPerson_T0EI_FieldName, "CONTACTPERSON", "$CONTACTPERSON");
        _fields[29] = new(MobileNo_2KZB_FieldName, "MOBILENUMBER", "$MOBILENUMBER");
        _fields[30] = new(PhoneNumber_KFKX_FieldName, "PHONENUMBER", "$PHONENUMBER");
        _fields[31] = new(FaxNumber_HQNP_FieldName, "FAXNUMBER", "$FAXNUMBER");
        _fields[32] = new(Email_S2QE_FieldName, "EMAIL", "$EMAIL");
        _fields[33] = new(PANNumber_1WOF_FieldName, "INCOMETAXNUMBER", "$INCOMETAXNUMBER");
        _fields[34] = new(VATNumber_D5F0_FieldName, "VATTINNUMBER", "$VATTINNUMBER");
        _fields[35] = new(CSTNumber_PWWU_FieldName, "INTERSTATESTNUMBER", "$INTERSTATESTNUMBER");
        _fields[36] = new(ExciseNatureOfPurchase_UV2K_FieldName, "EXCISENATUREOFPURCHASE", "$EXCISENATUREOFPURCHASE");
        _fields[37] = new(ExciseRegistrationNo_LLVU_FieldName, "EXCISEREGNO", "$EXCISEREGNO");
        _fields[38] = new(ExciseImportRegistrationNo_5EMT_FieldName, "EXCISEIMPORTSREGISTARTIONNO", "$EXCISEIMPORTSREGISTARTIONNO");
        _fields[39] = new(ImportExportCode_2JP6_FieldName, "IMPORTEREXPORTERCODE", "$IMPORTEREXPORTERCODE");
        _fields[40] = new(GSTDealerType_MAMX_FieldName, "GSTREGISTRATIONTYPE", "$GSTREGISTRATIONTYPE");
        _fields[41] = new(IsOtherTerritoryAssessee_IKOQ_FieldName, "ISOTHTERRITORYASSESSEE", "$ISOTHTERRITORYASSESSEE");
        _fields[42] = new(GSTIN_7NPB_FieldName, "PARTYGSTIN", "$PARTYGSTIN");
        _fields[43] = new(ApplicableFrom_Q1D6_FieldName, "APPLICABLEFROM", "$APPLICABLEFROM");
        _fields[44] = new(Range_CZ2U_FieldName, "RANGE", "$RANGE");
        _fields[45] = new(Division_SL2I_FieldName, "DIVISION", "$DIVISION");
        _fields[46] = new(Commissionerate_4I3E_FieldName, "COMMISSIONERATE", "$COMMISSIONERATE");
        _fields[47] = new(AdressLines_C4G4_FieldName, "ADDRESS", "$ADDRESS");
        _fields[48] = new(ApplicableFrom_QLJQ_FieldName, "APPLICABLEFROM", "$APPLICABLEFROM");
        _fields[49] = new(MailingName_ZIWT_FieldName, "MAILINGNAME", "$MAILINGNAME");
        _fields[50] = new(Country_YDYN_FieldName, "COUNTRY", "$COUNTRY");
        _fields[51] = new(State_FSSQ_FieldName, "STATE", "$STATE");
        _fields[52] = new(PINCode_SFTL_FieldName, "PINCODE", "$PINCODE");
        _fields[53] = new(ApplicableFrom_IJLE_FieldName, "APPLICABLEFROM", "$APPLICABLEFROM");
        _fields[54] = new(GSTRegistrationType_KGBW_FieldName, "GSTREGISTRATIONTYPE", "$GSTREGISTRATIONTYPE");
        _fields[55] = new(State_PJCH_FieldName, "STATE", "$STATE");
        _fields[56] = new(PlaceOfSupply_QR0I_FieldName, "PLACEOFSUPPLY", "$PLACEOFSUPPLY");
        _fields[57] = new(IsOtherTerritoryAssesse_EZRV_FieldName, "ISOTHTERRITORYASSESSEE", "$ISOTHTERRITORYASSESSEE");
        _fields[58] = new(ConsiderPurchaseForExport_DLDG_FieldName, "CONSIDERPURCHASEFOREXPORT", "$CONSIDERPURCHASEFOREXPORT");
        _fields[59] = new(IsTransporter_JL8F_FieldName, "ISTRANSPORTER", "$ISTRANSPORTER");
        _fields[60] = new(TransporterId_MJQC_FieldName, "TRANSPORTERID", "$TRANSPORTERID");
        _fields[61] = new(IsCommonParty_YTBM_FieldName, "ISCOMMONPARTY", "$ISCOMMONPARTY");
        _fields[62] = new(GSTIN_JA0L_FieldName, "GSTIN", "$GSTIN");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, "LEDGER", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return["REMOTEALTGUID", "ALTERID", "ENTEREDBY", "ALTEREDBY", "NAME", "Alias", "PARENT", "CURRENCY", "TAXTYPE", "GSTTYPE", "RATEOFTAXCALCULATION", "APPROPRIATEFOR", "ISBILLWISEON", "LanguageName.NAME,LanguageName.LANGUAGEID", "OPENINGBALANCE", "LEDMULTIADDRESSLIST.MAILINGNAME,LEDMULTIADDRESSLIST.ADDRESS,LEDMULTIADDRESSLIST.COUNTRYNAME,LEDMULTIADDRESSLIST.LEDSTATENAME,LEDMULTIADDRESSLIST.PINCODE,LEDMULTIADDRESSLIST.CONTACTPERSON,LEDMULTIADDRESSLIST.MOBILENUMBER,LEDMULTIADDRESSLIST.PHONENUMBER,LEDMULTIADDRESSLIST.FAXNUMBER,LEDMULTIADDRESSLIST.EMAIL,LEDMULTIADDRESSLIST.INCOMETAXNUMBER,LEDMULTIADDRESSLIST.VATTINNUMBER,LEDMULTIADDRESSLIST.INTERSTATESTNUMBER,LEDMULTIADDRESSLIST.EXCISENATUREOFPURCHASE,LEDMULTIADDRESSLIST.EXCISEREGNO,LEDMULTIADDRESSLIST.EXCISEIMPORTSREGISTARTIONNO,LEDMULTIADDRESSLIST.IMPORTEREXPORTERCODE,LEDMULTIADDRESSLIST.GSTREGISTRATIONTYPE,LEDMULTIADDRESSLIST.ISOTHTERRITORYASSESSEE,LEDMULTIADDRESSLIST.PARTYGSTIN", "LEDMAILINGDETAILS.ADDRESS,LEDMAILINGDETAILS.APPLICABLEFROM,LEDMAILINGDETAILS.MAILINGNAME,LEDMAILINGDETAILS.COUNTRY,LEDMAILINGDETAILS.STATE,LEDMAILINGDETAILS.PINCODE", "LEDGSTREGDETAILS.APPLICABLEFROM,LEDGSTREGDETAILS.GSTREGISTRATIONTYPE,LEDGSTREGDETAILS.STATE,LEDGSTREGDETAILS.PLACEOFSUPPLY,LEDGSTREGDETAILS.ISOTHTERRITORYASSESSEE,LEDGSTREGDETAILS.CONSIDERPURCHASEFOREXPORT,LEDGSTREGDETAILS.ISTRANSPORTER,LEDGSTREGDETAILS.TRANSPORTERID,LEDGSTREGDETAILS.ISCOMMONPARTY,LEDGSTREGDETAILS.GSTIN", "LEDMULTIADDRESSLIST.EXCISEJURISDICTIONDETAILS.APPLICABLEFROM,LEDMULTIADDRESSLIST.EXCISEJURISDICTIONDETAILS.RANGE,LEDMULTIADDRESSLIST.EXCISEJURISDICTIONDETAILS.DIVISION,LEDMULTIADDRESSLIST.EXCISEJURISDICTIONDETAILS.COMMISSIONERATE"];
    }
}