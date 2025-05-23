//-----------------------------------------------------------------------
// <copyright file="TypeScriptPropertyNameGenerator.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NJsonSchema.CodeGeneration.TypeScript
{
    /// <summary>Generates the property name for a given TypeScript <see cref="JsonSchemaProperty"/>.</summary>
    public sealed class TypeScriptPropertyNameGenerator : IPropertyNameGenerator
    {
        private const string FirstPassChars = "\"@?.=+";
#if NET8_0_OR_GREATER
        private static readonly System.Buffers.SearchValues<char> _reservedFirstPassChars = System.Buffers.SearchValues.Create(FirstPassChars);
#else
        private static readonly char[] _reservedFirstPassChars = FirstPassChars.ToCharArray();
#endif

        private const string SecondPassChars = "*#:-";
#if NET8_0_OR_GREATER
        private static readonly System.Buffers.SearchValues<char> _reservedSecondPassChars = System.Buffers.SearchValues.Create(SecondPassChars);
#else
        private static readonly char[] _reservedSecondPassChars = SecondPassChars.ToCharArray();
#endif

        /// <summary>Gets or sets the reserved names.</summary>
        public HashSet<string> ReservedPropertyNames { get; set; } = new(StringComparer.Ordinal) { "constructor", "init", "fromJS", "toJSON" };

        /// <inheritdoc />
        public string Generate(JsonSchemaProperty property)
        {
            var name = property.Name;

            if (name.AsSpan().IndexOfAny(_reservedFirstPassChars) != -1)
            {
                name = name.Replace("\"", string.Empty)
                    .Replace("@", string.Empty)
                    .Replace("?", string.Empty)
                    .Replace(".", "-")
                    .Replace("=", "-")
                    .Replace("+", "plus");
            }

            name = ConversionUtilities.ConvertToLowerCamelCase(name, true);

            if (name.AsSpan().IndexOfAny(_reservedSecondPassChars) != -1)
            {
                name = name.Replace("*", "Star")
                    .Replace("#", "_")
                    .Replace(":", "_")
                    .Replace("-", "_");
            }

            if (ReservedPropertyNames.Contains(name))
            {
                return name + "_";
            }

            return name;
        }
    }
}
