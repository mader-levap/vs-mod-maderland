# Trample feature

## How it works

When player walks on block, mod checks if that block is in config. If it is, it reduces durability of that block by configured amount. If durability hits 0, block changes to another block (configured in `ToBlockCode`) or gets removed if `ToBlockCode` is empty.

Some trampled blocks cannot change back to original blocks. So grass on soil will not regrow as long as trample data is present and Durability is not at max.

## Commands

- `/ml trample active [on|off]`: Turn on/off Trample feature. You can skip `[on/off]`, will flip. If `off`, will completely disable mod, freezing state of trampling data.
- `/ml trample allow [on|off]`: If `off`, mod will not reduce durability of blocks, but will still handle any existing trampling data.

## Configuration

Format of config file `maderland/trample.json`:

- `Active`: Can turn on/off Trample feature.
- `Power`: Power of trampling. All of these will lower Durability of blocks.
  - `PlayerBarefoot`: How powerful is walking barefoot. Note: completely destroyed shoes/boots count as barefoot.
  - `PlayerShoes`: How powerful is walking using shoes.
  - `PlayerArmored`: How powerful is walking using armored boots.
- `Passable`: two lists of passable blocks, like `game:tallgrass-medium-free`.
- `Impassable`: two lists of impassable blocks, like `game:soil-*-normal`.

Format for passable/impassable:

- `Blocks`: Dictionary of blocks. Key is source block code, value is trampling data. Block code must be exact. Format of entry:
  - `ToBlockCode`: Target block code. Can be empty if should remove block completely.
  - `Durability`: Durability of block. Walking on this block will reduce that number. If it hits 0, block changes to ToBlockCode.
  - `Regen`: Regeneration of block. If block is not walked on, it will regenerate durability by this amount per second. If you want to disable regeneration, set it to 0.

Example:
```
  "Blocks": {
    "game:soil-sparse": {
      "ToBlockCode": "game:soil-verysparse",
      "Durability": 50.0,
      "Regen": 10.0
    },
    "game:soil-verysparse": {
      "ToBlockCode": "",
      "Durability": 50.0,
      "Regen": 10.0
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
      "Regen": 10.0
    }
  ]
```
Format of entry is same as `Blocks`, only difference is addition of `FromBlockCode` field. Note you can have ToBlockCode without wildcard (will replace all variants with same block) or empty (will remove block).

You can specify special state where FromBlockCode is same as ToBlockCode. It won't change block, but will still reduce durability with all side effects like grass being unable to grow back.