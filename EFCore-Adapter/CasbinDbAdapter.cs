using NetCasbin.Persist;
using NetCasbin.Model;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace Casbin.NET.Adapter.EFCore
{
    public class CasbinDbAdapter : IAdapter
    {
        private readonly CasbinDbContext _context;

        public CasbinDbAdapter(CasbinDbContext context)
        {
            _context = context;
        }

        public void LoadPolicy(Model model)
        {
            var rules = _context.CasbinRule.ToList();
            LoadPolicyData(model, Helper.LoadPolicyLine, rules);
        }

        public void RemovePolicy(string sec, string pType, IList<string> rule)
        {
            RemoveFilteredPolicy(sec, pType, 0, rule.ToArray());
        }

        public void RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (fieldValues == null || !fieldValues.Any())
                return;
            var line = new CasbinRule(){
                PType = ptype
            };
            var len = fieldValues.Count();
            if (fieldIndex <= 0 && 0 < fieldIndex + len)
            {
                line.V0 = fieldValues[0 - fieldIndex];
            }
            if (fieldIndex <= 1 && 1 < fieldIndex + len)
            {
                line.V1 = fieldValues[1 - fieldIndex];
            }
            if (fieldIndex <= 2 && 2 < fieldIndex + len)
            {
                line.V2 = fieldValues[2 - fieldIndex];
            }
            if (fieldIndex <= 3 && 3 < fieldIndex + len)
            {
                line.V3 = fieldValues[3 - fieldIndex];
            }
            if (fieldIndex <= 4 && 4 < fieldIndex + len)
            {
                line.V4 = fieldValues[4 - fieldIndex];
            }
            if (fieldIndex <= 5 && 5 < fieldIndex + len)
            {
                line.V5 = fieldValues[5 - fieldIndex];
            }
            var dbRow = _context.CasbinRule.Where(p => p.PType == line.PType)
               .Where(p => p.V0 == line.V0)
               .Where(p => p.V1 == line.V1)
               .Where(p => p.V2 == line.V2)
               .Where(p => p.V3 == line.V3)
               .Where(p => p.V4 == line.V4)
               .Where(p => p.V5 == line.V5)
               .FirstOrDefault();
       
            if (dbRow != null)
            {
                _context.Entry(dbRow).State = EntityState.Deleted;
                _context.SaveChanges();
            }
        }

        public void SavePolicy(Model model)
        {
            List<CasbinRule> lines = new List<CasbinRule>();
            if (model.Model.ContainsKey("p"))
            {
                foreach (var kv in model.Model["p"])
                {
                    var ptype = kv.Key;
                    var ast = kv.Value;
                    foreach (var rule in ast.Policy)
                    {
                        var line = savePolicyLine(ptype, rule);
                        lines.Add(line);
                    }
                }
            }
            if (model.Model.ContainsKey("g"))
            {
                foreach (var kv in model.Model["g"])
                {
                    var ptype = kv.Key;
                    var ast = kv.Value;
                    foreach (var rule in ast.Policy)
                    {
                        var line = savePolicyLine(ptype, rule);
                        lines.Add(line);
                    }
                }
            }
            if (lines.Any())
            {
                _context.CasbinRule.AddRange(lines);
                _context.SaveChanges();
            }
        }

        public void AddPolicy(string sec, string pType, IList<string> rule)
        {
            var line = savePolicyLine(pType, rule);
            _context.CasbinRule.Add(line);
            _context.SaveChanges();
        }

        private void LoadPolicyData(Model model, Helper.LoadPolicyLineHandler<string, Model> handler, IEnumerable<CasbinRule> rules)
        {
            foreach (var rule in rules)
            {
                handler(GetPolicyCotent(rule), model);
            }
        }

        private string GetPolicyCotent(CasbinRule rule)
        {
            StringBuilder sb = new StringBuilder(rule.PType);
            void Append(string v)
            {
                if (string.IsNullOrEmpty(v))
                {
                    return;
                }
                sb.Append($", {v}");
            }
            Append(rule.V0);
            Append(rule.V1);
            Append(rule.V2);
            Append(rule.V3);
            Append(rule.V4);
            Append(rule.V5);
            return sb.ToString();
        }

        private CasbinRule savePolicyLine(string pType, IList<string> rule)
        {
            var line = new CasbinRule();
            line.PType = pType;
            if (rule.Count() > 0)
            {
                line.V0 = rule[0];
            }
            if (rule.Count() > 1)
            {
                line.V1 = rule[1];
            }
            if (rule.Count() > 2)
            {
                line.V2 = rule[2];
            }
            if (rule.Count() > 3)
            {
                line.V3 = rule[3];
            }
            if (rule.Count() > 4)
            {
                line.V4 = rule[4];
            }
            if (rule.Count() > 5)
            {
                line.V5 = rule[5];
            }

            return line;
        }
    }
}
