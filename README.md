# Test File Generator
## Description
- Creates a text file of the described format
- Allows specifying the file size

## Usage
From the project folder call this:
`dotnet run --project ./LargeFileSorter.FileGenerator.ConsoleClient/LargeFileSorter.FileGenerator.ConsoleClient.csproj [-d|--dir <directory>] [-f|--file <fileName>] [-s|--size <fileSize>]`

### -d | -dir <directory>
Defines the output directory. By default, the current directory.

### -f | -file <fileName>
Defines the output file name. By default `largeFile.txt`

### -s | -size <fileSize>
Defines the output file size. By default 512Mb. (Examples: 5Gb, 1Tb).

# Sorter
## Description
- Sorts the file according to the given criteria
  - First, sort by the string part (alphabetically)
  - If two lines have the same string, sort by the number (ascending)
- Must handle very large files (~100GB) efficiently

Example Input:
```
415. Apple
30432. Something something something
1. Apple
32. Cherry is the best
2. Banana is yellow
```

Expected Output:
```
1. Apple
415. Apple
2. Banana is yellow
32. Cherry is the best
30432. Something something something
```

## Usage
From the project folder call this:
`dotnet run --project ./LargeFileSorter.FileSorterer.ConsoleClient/LargeFileSorter.FileSorterer.ConsoleClient.csproj <inputFile> [-d|--dir <directory>] [-f|--file <fileName>]`

### <inputFile>
Required parameter. Defines the input file.

### -d | -dir <directory>
Defines the output directory. By default, the current directory.

### -f | -file <fileName>
Defines the output file name. By default name of the input file with the prefix `Sorted_`
