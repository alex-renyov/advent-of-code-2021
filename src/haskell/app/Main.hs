{-# LANGUAGE QuasiQuotes #-}

module Main where

import Text.RawString.QQ

main :: IO ()
main = putStrLn multiline

multiline :: String
multiline = [r|123
456
789
|]