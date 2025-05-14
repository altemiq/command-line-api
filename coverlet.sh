#!/bin/bash

root_path=$(readlink -f $(dirname "$0"))

# remove any current values
results=$root_path/coverage/results
if [ -d "$results" ]; then
  rm -rf $results
fi

# Run dotnet test
dotnet test $(dirname "$0")/src --no-build -- --results-directory $results --coverage --coverage-output-format cobertura

# install the report generator
dotnet tool install -g dotnet-reportgenerator-globaltool

reports=$root_path/coverage/reports
if [ -d "$reports" ]; then
  rm -rf $reports
fi

# run the report generator
reportgenerator -reports:"$results/*.cobertura.xml" -targetdir:$reports -reporttypes:"HtmlInline;Cobertura;MarkdownSummary" -verbosity:Verbose
