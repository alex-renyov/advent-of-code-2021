module Day2(part1, part2) where

import qualified Day2Input

data Command = Forward Int | Down Int | Up Int

data Boat1 = Boat1 Int Int
data Boat2 = Boat2 Int Int Int

part1 :: Int
part1 = pos * depth
                where moveBoat :: Boat1 -> Command -> Boat1
                      moveBoat (Boat1 pos depth) (Forward value) = Boat1 (pos + value) depth
                      moveBoat (Boat1 pos depth) (Down value) = Boat1 pos (depth + value)
                      moveBoat (Boat1 pos depth) (Up value) = Boat1 pos (depth - value)
                      initialBoat = Boat1 0 0
                      Boat1 pos depth = foldl moveBoat initialBoat inputCommands

part2 :: Int
part2 = pos * depth
              where moveBoat :: Boat2 -> Command -> Boat2
                    moveBoat (Boat2 pos depth aim) (Forward value) = Boat2 (pos + value) (depth + aim * value) aim
                    moveBoat (Boat2 pos depth aim) (Down value) = Boat2 pos depth (aim + value)
                    moveBoat (Boat2 pos depth aim) (Up value) = Boat2 pos depth (aim - value)
                    initialBoat = Boat2 0 0 0
                    Boat2 pos depth aim = foldl moveBoat initialBoat inputCommands

inputCommands :: [Command]
inputCommands = map parseCommand inputLines
                    where mapCommand "forward" = Forward
                          mapCommand "down"    = Down
                          mapCommand "up"      = Up
                          makeCommand [name, value] = mapCommand name $ read value
                          parseCommand = makeCommand . words

inputLines :: [String]
inputLines = lines Day2Input.inputValues
