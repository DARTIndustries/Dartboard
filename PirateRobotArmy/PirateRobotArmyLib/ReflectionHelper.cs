using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PirateRobotArmyLib
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Tuple<TypeInfo, TeamInfoAttribute>> FindAllAis(Assembly target)
        {
            List<Tuple<TypeInfo, TeamInfoAttribute>> results = new List<Tuple<TypeInfo, TeamInfoAttribute>>();
            try
            {
                foreach (var typeinfo in target.DefinedTypes)
                {
                    var customAttr = typeinfo.GetCustomAttribute<TeamInfoAttribute>(true);
                    if (customAttr != null && typeof(TeamAi).IsAssignableFrom(typeinfo))
                    {
                        results.Add(Tuple.Create(typeinfo, customAttr));
                    }
                }
            }catch(Exception e)
            {
                return new List<Tuple<TypeInfo, TeamInfoAttribute>>();
            }
            return results;
        }
    }
}
