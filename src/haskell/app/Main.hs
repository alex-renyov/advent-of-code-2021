module Main where

import qualified Day1
import qualified Day2

main :: IO ()
main = do
          putStrLn "Day 1"
          putStrLn $ show Day1.part1
          putStrLn $ show Day1.part2
          putStrLn "Day 2"
          putStrLn $ show Day2.part1
          putStrLn $ show Day2.part2

