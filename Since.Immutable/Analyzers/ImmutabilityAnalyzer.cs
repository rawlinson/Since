using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Since.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ImmutabilityAnalyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor NonImmutableBaseRule = new DiagnosticDescriptor("SIA1001",
            new LocalizableResourceString(nameof(Resources.NonImmutableBaseTitle), Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.NonImmutableBaseMessage), Resources.ResourceManager, typeof(Resources)),
            "Immutability", DiagnosticSeverity.Error, true);

        private static DiagnosticDescriptor NonImmutableMemberAccessRule = new DiagnosticDescriptor("SIA1002",
            new LocalizableResourceString(nameof(Resources.NonImmutableMemberAccessTitle), Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.NonImmutableMemberAccessMessage), Resources.ResourceManager, typeof(Resources)),
            "Immutability", DiagnosticSeverity.Error, true);

        private static DiagnosticDescriptor NonImmutableMemberTypeRule = new DiagnosticDescriptor("SIA1003",
            new LocalizableResourceString(nameof(Resources.NonImmutableMemberTypeTitle), Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.NonImmutableMemberTypeMessage), Resources.ResourceManager, typeof(Resources)),
            "Immutability", DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(NonImmutableBaseRule, NonImmutableMemberAccessRule, NonImmutableMemberTypeRule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;

            bool trusted;
            if (IsMarkedImmutable(type, out trusted) && !trusted)
                ValidateIsImmutable(type, context);
        }
        static bool IsMarkedImmutable(ISymbol symbol, out bool trusted)
        {
            trusted = false;
            foreach (var attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass.Name == "ImmutableAttribute")
                {
                    foreach (var arg in attribute.NamedArguments)
                    {
                        if (arg.Key == "Trusted" && !arg.Value.IsNull)
                            trusted = arg.Value.Value.Equals(true);
                    }

                    return true;
                }
            }
            return false;
        }

        private static HashSet<string> ImmutableTypes = new HashSet<string>
        {
            "Object", "String",
            "DateTime", "TimeSpan",
            "IEnumerable", "IEnumerator",
            "IReadOnlyCollection", "IReadOnlyDictionary", "IReadOnlyList"
        };

        private static bool IsImmutable(INamedTypeSymbol type)
        {
            if (ImmutableTypes.Contains(type.Name))
                return true;

            if (type.IsValueType)
                return true;
            
            bool _;
            return IsMarkedImmutable(type, out _);
        }

        private static void ValidateIsImmutable(INamedTypeSymbol type, SymbolAnalysisContext context)
        {
            var baseTypeIsImmutable = type.BaseType != null ? IsImmutable(type.BaseType as INamedTypeSymbol) : true;
            if (baseTypeIsImmutable == false)
            {
                context.ReportDiagnostic(Diagnostic.Create(NonImmutableBaseRule, type.Locations[0], type.Name, type.BaseType.Name));
            }

            foreach (var member in type.GetMembers())
            {
                bool trustedImmutable;
                if (IsMarkedImmutable(member, out trustedImmutable) && trustedImmutable)
                    continue;

                if (member.Kind == SymbolKind.Field)
                {
                    var field = member as IFieldSymbol;
                    if (field.AssociatedSymbol is IPropertySymbol)
                        continue;

                    if (!(field.IsReadOnly || field.IsConst))
                    {
                        //context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberAccessRule, type.Locations[0], type.Name, field.Name));
                        context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberAccessRule, field.Locations[0], type.Name, field.Name));
                    }

                    if (field.Type == type && !IsImmutable(field.Type as INamedTypeSymbol))
                    {
                        //context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberTypeRule, type.Locations[0], type.Name, field.Type.Name));
                        context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberTypeRule, field.Locations[0], type.Name, field.Type.Name));
                    }
                }
                else if (member.Kind == SymbolKind.Property)
                {
                    var property = member as IPropertySymbol;
                    if (!property.IsReadOnly)
                    {
                        //context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberAccessRule, type.Locations[0], type.Name, property.Name));
                        context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberAccessRule, property.Locations[0], type.Name, property.Name));
                    }

                    if (property.Type != type && !IsImmutable(property.Type as INamedTypeSymbol))
                    {
                        //context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberTypeRule, type.Locations[0], type.Name, property.Type.Name));
                        context.ReportDiagnostic(Diagnostic.Create(NonImmutableMemberTypeRule, property.Locations[0], type.Name, property.Type.Name));
                    }
                }
            }
        }

        /*

        static bool SetImmutable(string name, bool immutable)
            => ImmutableTypes2[name] = immutable;


        static bool IsImmutable(INamedTypeSymbol symbol, out string bad, bool forceCheck = false)
        {
            bad = null;

            if (ImmutableTypes.ContainsKey(symbol.Name))
                return ImmutableTypes[symbol.Name];
            if (ImmutableTypes2.ContainsKey(symbol.Name))
                return ImmutableTypes2[symbol.Name];

            if (!forceCheck)
            {
                foreach (var attribute in symbol.GetAttributes())
                {
                    if (attribute.AttributeClass.Name == "ImmutableAttribute")
                    {
                        if (attribute.ConstructorArguments.Length > 0)
                        {
                            return SetImmutable(symbol.Name, attribute.ConstructorArguments.First().Value.Equals(true));
                        }
                        return SetImmutable(symbol.Name, true);
                    }
                }
            }

            if (symbol.IsValueType && symbol.SpecialType != SpecialType.System_Enum)
                return SetImmutable(symbol.Name, true);

            foreach (var member in symbol.GetMembers())
            {
                if (member.Kind == SymbolKind.Field)
                {
                    var field = member as IFieldSymbol;

                    if (field.AssociatedSymbol is IPropertySymbol)
                        continue;

                    if (!(field.IsReadOnly || field.IsConst))
                    {
                        bad = field.Name;
                        return SetImmutable(symbol.Name, false);
                    }

                    var type = field.Type;
                    if (type == symbol)
                        continue;
                    if (!IsImmutable(type as INamedTypeSymbol, out bad, forceCheck))
                    {
                        bad = field.Name + "." + bad;
                        return SetImmutable(symbol.Name, false);
                    }

                    continue;
                }

                if (member.Kind == SymbolKind.Property)
                {
                    var property = member as IPropertySymbol;
                    if (!property.IsReadOnly)
                    {
                        bad = property.Name;
                        return SetImmutable(symbol.Name, false);
                    }

                    var type = property.Type;
                    if (type == symbol)
                        continue;
                    if (!IsImmutable(type as INamedTypeSymbol, out bad, forceCheck))
                    {
                        bad = property.Name + "." + bad;
                        return SetImmutable(symbol.Name, false);
                    }

                    continue;
                }
            }

            var baseTypeIsImmutable = symbol.BaseType != null ? IsImmutable(symbol.BaseType, out bad, forceCheck) : true;
            if (baseTypeIsImmutable == false)
                bad = $"base({symbol.BaseType.Name})" + "." + bad;

            return SetImmutable(symbol.Name, baseTypeIsImmutable);
        }
        */
    }
}