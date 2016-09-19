# Shaman.CommonExtensions
Commonly-used utility methods.

```csharp
using Shaman;

"something(1)".CaptureBetween("(", ")"); // returns "1"
something.RecursiveEnumeration(x => x.Parent); // gets the root
"something".RegexReplace("[aeiou]", "_"); // returns "s_m_th_ng"
"something".TrimStart("some"); // returns "thing"
"something".TrimEnd("thing"); // returns "some"
"something".Capture("s(.*)g"); // returns "omethin"

"something".In(arr); // like arr.Contains("something")
```