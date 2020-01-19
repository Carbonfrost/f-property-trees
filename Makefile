.PHONY: dotnet/generate dotnet/test

PUBLISH_DIR=dotnet/test/Carbonfrost.UnitTests.PropertyTrees/bin/$(CONFIGURATION)/netcoreapp3.0/publish

## Generate generated code
dotnet/generate:
	srgen -c Carbonfrost.Commons.PropertyTrees.Resources.SR \
		-r Carbonfrost.Commons.PropertyTrees.Automation.SR \
		--resx \
		dotnet/src/Carbonfrost.Commons.PropertyTrees/Automation/SR.properties

## Execute dotnet unit tests
dotnet/test: dotnet/publish -dotnet/test

-dotnet/test:
	@ fspec -i dotnet/test/Carbonfrost.UnitTests.PropertyTrees/Content \
		$(PUBLISH_DIR)/Carbonfrost.Commons.Core.dll \
		$(PUBLISH_DIR)/Carbonfrost.Commons.Core.Runtime.Expressions.dll \
		$(PUBLISH_DIR)/Carbonfrost.Commons.PropertyTrees.dll \
		$(PUBLISH_DIR)/Carbonfrost.UnitTests.PropertyTrees.dll

## Run unit tests with code coverage
dotnet/cover: dotnet/publish -check-command-coverlet
	coverlet \
		--target "make" \
		--targetargs "-- -dotnet/test" \
		--format lcov \
		--output lcov.info \
		--exclude-by-attribute 'Obsolete' \
		--exclude-by-attribute 'GeneratedCode' \
		--exclude-by-attribute 'CompilerGenerated' \
		$(PUBLISH_DIR)/Carbonfrost.UnitTests.PropertyTrees.dll


include eng/.mk/*.mk
