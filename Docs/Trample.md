# Trample feature

## How it works

When player walks on block, mod checks if that block is in config. If it is, it reduces durability of that block by configured amount. If durability hits 0, block changes to another block (configured in `ToBlockCode`) or gets removed if `ToBlockCode` is empty.

Some trampled blocks cannot change back to original blocks. So grass on soil will not regrow as long as trample data is present and Durability is not at max.

Blocks can regenerate and recover, allowing them to act normally, like growing grass on soil.

## Commands

- `/ml trample active [on|off]`: Turn on/off Trample feature. You can skip `[on/off]`, will flip. If `off`, will completely disable mod, freezing state of trampling data.
- `/ml trample allow [on|off]`: If `off`, mod will not reduce durability of blocks, but will still handle any existing trampling data.
- `/ml trample debug [on|off]`: If `on`, mod will show various debug information in window.

## Configuration

Format of config file `maderland/trample.json`:

- `Active`: Can turn on/off Trample feature.
- `Allow`: Can trample?
- `Debug`: Is in debug mode?
- `Fall`: Fall config. Note that trample power due to fall is in addition to normal trample power.
  - `CapHeight`: If larger than zero, fall height will be capped to this value.
  - `MinimumHeight`: Fall height smaller than this will be ignored. Effective fall height will be subtracted.
  - `Mul`: Basic multiplier of trampling power. Do not forget that entities also have their own individual `FallMul`.
- `Entities`: List of entities that can trample blocks. Each entry contains these fields:
  - `EntityCode`: Code of entity. You can use wildcards. Remember to put more specific code first, for example `game:deer-elk-adult-*` before `game:deer-*-adult-*`.
  - `Power`: Trampling power for just walking onto trampleable block.
  - `FallMul`: Multiplier for additional trampling power generated due to fall. Set to zero to disable that.
- `Passable`: two lists of passable blocks, like `game:tallgrass-medium-*`.
- `Impassable`: two lists of impassable blocks, like `game:soil-*-normal`.

Format for passable/impassable:

- `Blocks`: Dictionary of blocks. Key (`FromBlockCode`) is source block code, value is trampling data. Block code must be exact. Format of entry:
  - `ToBlockCode`: Target block code. Can be empty if should remove block completely.
  - `Durability`: Durability of block. Walking on this block will reduce that number. If it hits 0, block changes to ToBlockCode.
  - `Regen`: In-game days needed for block to fully regenerate all durability points from zero. Set to 0 to disable regeneration.
  - `DurRatio`: Durability ratio to apply when this block is placed due to trampling.
    For example, if block was placed in worldgen or due to player placing this block, its `Durability` will be same as in config and considered maximum.
    But if this block was placed because previous block was fully trampled, `Durability` will be multiplied by this ratio.

Example:
```
  "Blocks": {
    "game:soil-sparse": {
      "ToBlockCode": "game:soil-verysparse",
      "Durability": 50.0,
      "Regen": 10.0,
      "DurRatio": 0.5
    },
    "game:soil-verysparse": {
      "ToBlockCode": "",
      "Durability": 50.0,
      "Regen": 10.0,
      "DurRatio": 0.5
    }
  }
```
- `BlockVariants`: List of blocks. You use wildcard like you would use it elsewhere.

Example:
```
  "BlockVariants": [
    {
      "FromBlockCode": "game:soil-*-normal",
      "ToBlockCode": "game:soil-*-sparse",
      "Durability": 50.0,
      "Regen": 10.0,
      "DurRatio": 0.5
    }
  ]
```
Format of entry is same as `Blocks`, only difference is addition of `FromBlockCode` field. Note you can have `ToBlockCode` without wildcard (will replace all variants with same block) or empty (will remove block).

You can specify special state where `FromBlockCode` is same as `ToBlockCode`. It won't change block, but will still reduce durability with all side effects like grass being unable to grow back.