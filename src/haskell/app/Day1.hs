module Day1(part1, part2) where

import Day1Input(inputValue)

part1 :: Int
part1 = increasesCount inputNumbers

part2 :: Int
part2 = increasesCount triplets
            where sumLists = zipWith (+)
                  triplets = foldr1 sumLists $ map drop [0..2] <*> [inputNumbers]

increasesCount :: [Int] -> Int
increasesCount values = length $ filter id $ zipWith (<) values $ drop 1 values

inputNumbers :: [Int]
inputNumbers = map read $ lines inputValue
