# Changelog

## [1.1.2] - 2026-10-21

### Fixed
- `SerializableGuid.GetHashCode` mixed only the first half of the guid, so guids that differ only in the second half collided
- `RuntimeBlackboard.TryGetVariable<T>` could throw `InvalidCastException` on a type mismatch instead of returning `false`
- Empty name guards used `string.IsNullOrWhiteSpace` on an already normalized name; they now use `string.IsNullOrEmpty`

## [1.1.1] - 2026-03-29

### Changed
- Renamed `Blackboard` class to `RuntimeBlackboard` to better distinguish from the overall concept/package name
- Updated all documentation and code examples to reflect the new class name

## [1.1.0] - 2026-03-29

### Added
- Added `BlackboardCategoryAttribute` for categorizing blackboard entries in the editor
- Added built-in extension methods directly to `Blackboard` class

### Changed
- Refactored `BlackboardRegistry` to support custom types with categories
- Improved README documentation with more examples

### Removed
- Removed `BlackboardExtensions` class (functionality merged into `Blackboard`)

## [1.0.0] - 2026-03-28

Initial release