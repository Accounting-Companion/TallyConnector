using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Models;

public class MastersMapping
{
    public MastersMapping(string masterType, string tallyMasterType, List<Filter> filters)
    {
        MasterType = masterType;
        Filters = filters;
        TallyMasterType = tallyMasterType;
    }

    public string MasterType { get; set; }
    public string TallyMasterType { get; set; }

    public List<Filter> Filters { get; set; }

    public static readonly List<MastersMapping> MastersMappings = new()
    {
        new MastersMapping("Currencies", "Currency", null),

        new MastersMapping("Groups", "Group", null),
        new MastersMapping("Ledgers", "Ledger", null),

        new MastersMapping("CostCategories", "CostCategory", null),
        new MastersMapping("CostCenters", "CostCentre",
            new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                new Filter("IsPayroll", "Not $FORPAYROLL")
            }),

        new MastersMapping("Units", "Unit", null),
        new MastersMapping("Godowns", "Godown", null),
        new MastersMapping("StockCategories", "StockCategory", null),
        new MastersMapping("StockGroups", "StockGroup", null),
        new MastersMapping("StockItems", "StockItem", null),

        new MastersMapping("AttendanceTypes", "AttendanceType", null),
        new MastersMapping("EmployeeGroups", "CostCentre", new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "$ISEMPLOYEEGROUP"),
            }),
        new MastersMapping("Employees", "CostCentre", new List<Filter>()
            {
                new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
                new Filter("IsPayroll", "$FORPAYROLL")
            }),

        new MastersMapping("VoucherTypes", "VoucherType", null),
    };
}

public class MastersBasicInfo<T>
{
    public MastersBasicInfo(string masterType, List<T> masters)
    {
        MasterType = masterType;
        Masters = masters;
    }

    public string MasterType { get; set; }

    public List<T> Masters { get; set; } = new();

    public int Count => Masters.Count;
}
