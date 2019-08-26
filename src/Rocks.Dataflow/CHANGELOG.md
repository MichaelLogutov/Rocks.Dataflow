## 2.6.2 - 2018-08-26
### Changed
- Removed bounded capacity restriction on after batch block (used only when max batch timeout set)

## 2.6.1 - 2018-08-25
### Fixed
- Fixed issue with batch timer not firing properly

## 2.6.0 - 2018-04-28
### Added
- Added Batch fluent method that allows building dataflow with BatchBlock with optional timeout on awaiting the total items in the batch 
- Added WithEnsureOrdered dataflow fluent method
### Changed
- Update packages
- Code cleanup and refactoring
- By default ExecutionDataflowBlockOptions.EnsureOrdered set to false

## 2.5.0 - 2018-04-28
### Removed
- Removed .NET 4.6.1

## 2.4.0 - 2018-04-27
### Changed
- Update packages