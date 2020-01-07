.PHONY: dotnet/generate dotnet/test

## Generate generated code
dotnet/generate:
	srgen -c Carbonfrost.Commons.PropertyTrees.Resources.SR \
		-r Carbonfrost.Commons.PropertyTrees.Automation.SR \
		--resx \
		dotnet/src/Carbonfrost.Commons.PropertyTrees/Automation/SR.properties

## Execute dotnet unit tests
dotnet/test: dotnet/publish
	fspec -i dotnet/test/Carbonfrost.UnitTests.PropertyTrees/Content \
		dotnet/test/Carbonfrost.UnitTests.PropertyTrees/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Core.dll \
		dotnet/test/Carbonfrost.UnitTests.PropertyTrees/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Core.Runtime.Expressions.dll \
		dotnet/test/Carbonfrost.UnitTests.PropertyTrees/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.PropertyTrees.dll \
		dotnet/test/Carbonfrost.UnitTests.PropertyTrees/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.PropertyTrees.dll

include eng/.mk/*.mk
