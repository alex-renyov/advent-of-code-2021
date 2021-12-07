module Day1(part1, part2) where

import Data.Functor
import qualified Day1Input

part1 :: Int
part1 = increasesCount inputNumbers

part2 :: Int
part2 = increasesCount triplets
            where sumLists = foldr1 $ zipWith (+)
                  triplets = sumLists $ [0..2] <&> flip drop inputNumbers

increasesCount :: [Int] -> Int
increasesCount values = length $ filter id $ zipWith (<) values $ drop 1 values

inputNumbers :: [Int]
inputNumbers = map read $ lines Day1Input.inputValues
